#nullable enable
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks;
using CommunityBikeSharing.Models;
using CommunityBikeSharing.Services.Data.Memberships;
using CommunityBikeSharing.Services.Data.Stations;
using Xamarin.Essentials;

namespace CommunityBikeSharing.Services.Data.Bikes
{
	public class BikeService : IBikeService
	{
		private readonly IDictionary<string, ObservableCollection<Bike>> _allBikes =
			new Dictionary<string, ObservableCollection<Bike>>();

		private readonly ObservableCollection<Bike> _bikes = new ObservableCollection<Bike>();

		private readonly NotifyCollectionChangedEventHandler _bikesChanged;

		private readonly IBikeRepository _bikeRepository;
		private readonly IAuthService _authService;
		private readonly IMembershipRepository _membershipRepository;
		private readonly ILocationService _locationService;
		private readonly IStationRepository _stationRepository;
		private string? _userId;

		public BikeService(
			IBikeRepository bikeRepository,
			IAuthService authService,
			IMembershipRepository membershipRepository,
			ILocationService locationService,
			IStationRepository stationRepository)
		{
			_bikeRepository = bikeRepository;
			_authService = authService;
			_membershipRepository = membershipRepository;
			_locationService = locationService;
			_stationRepository = stationRepository;

			_bikesChanged = (sender, args) =>
			{
				_bikes.Clear();

				foreach (var bike in _allBikes.Values
					.SelectMany(bikes => bikes)
					.Where(bike => (!bike.InUse && bike.StationId == null)
					               || bike.CurrentUser == _userId))
				{
					_bikes.Add(bike);
				}
			};
		}

		public ObservableCollection<Bike> ObserveBikesFromStation(Station station) =>
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
					bike.Value.CollectionChanged -= _bikesChanged;
				}

				_allBikes.Clear();

				foreach (var membership in communities)
				{
					var communityBikes =
						_bikeRepository.ObserveBikesFromCommunity(membership.CommunityId);

					communityBikes.CollectionChanged += _bikesChanged;
					_allBikes[membership.Id] = communityBikes;
				}
			};

			return _bikes;
		}

		public Task LendBike(Bike bike)
		{
			bike.CurrentUser = _userId;
			bike.StationId = null;
			bike.Location = null;
			return _bikeRepository.Update(bike);
		}

		public async Task ReturnBike(Bike bike)
		{
			bike.CurrentUser = null;

			var stations = await _stationRepository.GetStationsFromCommunity(bike.CommunityId);
			var location = await _locationService.GetCurrentLocation();

			var closeStation = stations.SingleOrDefault(station =>
				station.Location.CalculateDistance(location, DistanceUnits.Kilometers) < 0.1);

			if (closeStation != null)
			{
				bike.StationId = closeStation.Id;
			}
			else
			{
				bike.Location = location;
			}

			await _bikeRepository.Update(bike);
		}

		public Task<Bike> Add(string name, string communityId)
			=> _bikeRepository.Add(new Bike {Name = name, CommunityId = communityId});

		public Task Rename(Bike bike, string name)
		{
			bike.Name = name;
			return _bikeRepository.Update(bike);
		}

		public Task Delete(Bike bike) => _bikeRepository.Delete(bike);

		public ObservableCollection<Bike> ObserveBikesFromCommunity(string communityId) =>
			_bikeRepository.ObserveBikesFromCommunity(communityId);
	}
}

