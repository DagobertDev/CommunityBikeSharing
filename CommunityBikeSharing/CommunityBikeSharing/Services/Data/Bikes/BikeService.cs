using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks;
using CommunityBikeSharing.Models;
using CommunityBikeSharing.Models.Extensions;
using CommunityBikeSharing.Services.Data.Communities;
using CommunityBikeSharing.Services.Data.Locks;
using CommunityBikeSharing.Services.Data.Memberships;
using CommunityBikeSharing.Services.Data.Stations;
using Plugin.CloudFirestore;
using Xamarin.Essentials;

namespace CommunityBikeSharing.Services.Data.Bikes
{
	public class BikeService : IBikeService
	{
		private readonly IFirestoreContext _context;

		private readonly IDictionary<string, ObservableCollection<Bike>> _allBikes =
			new Dictionary<string, ObservableCollection<Bike>>();

		private readonly ObservableCollection<Bike> _bikes = new ObservableCollection<Bike>();

		private readonly IBikeRepository _bikeRepository;
		private readonly IAuthService _authService;
		private readonly IMembershipRepository _membershipRepository;
		private readonly ILocationService _locationService;
		private readonly IStationRepository _stationRepository;
		private readonly ICommunityRepository _communityRepository;
		private readonly ILockRepository _lockRepository;
		private readonly ILockService _lockService;
		private string? _userId;

		public BikeService(
			IFirestoreContext context,
			IBikeRepository bikeRepository,
			IAuthService authService,
			IMembershipRepository membershipRepository,
			ILocationService locationService,
			IStationRepository stationRepository,
			ICommunityRepository communityRepository,
			ILockRepository lockRepository,
			ILockService lockService)
		{
			_context = context;
			_bikeRepository = bikeRepository;
			_authService = authService;
			_membershipRepository = membershipRepository;
			_locationService = locationService;
			_stationRepository = stationRepository;
			_communityRepository = communityRepository;
			_lockRepository = lockRepository;
			_lockService = lockService;
		}

		public IObservable<ICollection<Bike>> ObserveBikesFromStation(Station station) =>
			_bikeRepository.ObserveBikesFromStation(station);

		public ObservableCollection<Bike> GetAvailableBikes()
		{
			var newUserId = _authService.GetCurrentUserId();

			if (_userId == newUserId)
			{
				return _bikes;
			}

			_userId = newUserId;

			var communities = _membershipRepository.ObserveMembershipsFromUser(_userId);

			communities.CollectionChanged += (sender, args) =>
			{
				foreach (var bike in _allBikes)
				{
					bike.Value.CollectionChanged -= BikesChanged;
				}

				_allBikes.Clear();

				foreach (var membership in communities)
				{
					var communityBikes =
						_bikeRepository.ObserveBikesFromCommunity(membership.CommunityId);

					communityBikes.CollectionChanged += BikesChanged;
					_allBikes[membership.Id] = communityBikes;
				}
			};

			void BikesChanged(object o, NotifyCollectionChangedEventArgs eventArgs)
			{
				_bikes.Clear();

				foreach (var bike in _allBikes.Values
					.SelectMany(bikes => bikes)
					.Where(bike => bike.IsFreeFloating()
					               && !bike.IsReserved()
					               || bike.CurrentUser == _userId))
				{
					_bikes.Add(bike);
				}
			}

			return _bikes;
		}

		public async Task LendBike(Bike bike)
		{
			if (bike.HasLock)
			{
				if (!await _lockService.OpenLock(bike))
				{
					return;
				}
			}

			bike.CurrentUser = _userId;
			bike.Location = null;
			bike.ReservedUntil = null;

			if (bike.StationId != null)
			{
				var station = await _stationRepository.Get(bike.CommunityId, bike.StationId);
				bike.StationId = null;

				await _context.RunTransactionAsync(transaction =>
				{
					_stationRepository.Update(station, nameof(Station.NumberOfBikes), FieldValue.Increment(-1), transaction);
					_bikeRepository.Update(bike, transaction);
				});
				return;
			}

			await _bikeRepository.Update(bike);
		}

		public Task ReserveBike(Bike bike)
		{
			return _context.RunTransactionAsync(transaction =>
			{
				var community = _communityRepository.Get(bike.CommunityId, transaction);

				bike.CurrentUser = _userId;
				bike.ReservedUntil = DateTime.UtcNow.Add(community.ReserveTime);

				_bikeRepository.Update(bike, transaction);
			});
		}

		public Task DeleteReservation(Bike bike)
		{
			return _context.RunTransactionAsync(transaction =>
			{
				_bikeRepository.Update(bike, nameof(Bike.ReservedUntil), null, transaction);
				_bikeRepository.Update(bike, nameof(Bike.CurrentUser), null, transaction);
			});
		}

		public async Task ReturnBike(Bike bike)
		{
			var location = await _locationService.GetCurrentLocation();

			if (location == null)
			{
				return;
			}

			if (bike.HasLock)
			{
				if (!await _lockService.CloseLock(bike))
				{
					return;
				}
			}

			bike.ReservedUntil = null;
			bike.CurrentUser = null;

			var stations = await _stationRepository.GetStationsFromCommunity(bike.CommunityId);

			var closeStation = stations.SingleOrDefault(station =>
				station.Location.CalculateDistance(location, DistanceUnits.Kilometers) < 0.1);

			if (closeStation != null)
			{
				bike.StationId = closeStation.Id;

				await _context.RunTransactionAsync(transaction =>
				{
					_stationRepository.Update(closeStation, nameof(Station.NumberOfBikes), FieldValue.Increment(1), transaction);
					_bikeRepository.Update(bike, transaction);
				});
				return;
			}

			bike.Location = location;

			await _bikeRepository.Update(bike);
		}

		public IObservable<Bike?> Observe(string community, string bike) => _bikeRepository.Observe(community, bike);

		public Task<Bike> Add(string name, string communityId)
			=> _bikeRepository.Add(new Bike {Name = name, CommunityId = communityId});

		public Task Rename(Bike bike, string name) => _bikeRepository.Update(bike, nameof(Bike.Name), name);

		public async Task UpdateLocation(Bike bike, Location location)
		{
			if (bike.Lent)
			{
				throw new ApplicationException("Can't set location of bike that's used");
			}

			Station? oldStation = null;

			if (bike.StationId != null)
			{
				oldStation = await _stationRepository.Get(bike.CommunityId, bike.StationId);
				bike.StationId = null;
			}
			
			var stations = await _stationRepository.GetStationsFromCommunity(bike.CommunityId);

			var newStation = stations.SingleOrDefault(station =>
				station.Location.CalculateDistance(location, DistanceUnits.Kilometers) < 0.1);

			await _context.RunTransactionAsync(transaction =>
			{
				if (oldStation != null)
				{
					_stationRepository.Update(oldStation, nameof(Station.NumberOfBikes), FieldValue.Increment(-1), transaction);
				}

				if (newStation == null)
				{
					bike.Location = location;
				}
				else
				{
					_stationRepository.Update(newStation, nameof(Station.NumberOfBikes), FieldValue.Increment(1), transaction);
					bike.StationId = newStation.Id;
				}

				_bikeRepository.Update(bike, transaction);
			});
		}

		public async Task Delete(Bike bike)
		{
			Station? station = null;

			if (bike.StationId != null)
			{
				station = await _stationRepository.Get(bike.CommunityId, bike.StationId);
			}

			Lock? @lock = null;

			if (bike.HasLock)
			{
				@lock = await _lockRepository.Get(bike.CommunityId, bike.LockId!);
			}

			await _context.RunTransactionAsync(transaction =>
			{
				if (station != null)
				{
					_stationRepository.Update(station, nameof(Station.NumberOfBikes), FieldValue.Increment(-1), transaction);
				}
				if (@lock != null)
				{
					_lockRepository.Delete(@lock, transaction);
				}

				_bikeRepository.Delete(bike, transaction);
			});
		}

		public ObservableCollection<Bike> ObserveBikesFromCommunity(string communityId) =>
			_bikeRepository.ObserveBikesFromCommunity(communityId);
	}
}

