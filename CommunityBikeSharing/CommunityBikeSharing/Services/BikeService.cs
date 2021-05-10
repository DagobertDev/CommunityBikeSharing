using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks;
using CommunityBikeSharing.Models;
using CommunityBikeSharing.Services.Data;

namespace CommunityBikeSharing.Services
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
		private string _userId;

		public BikeService(
			IBikeRepository bikeRepository,
			IAuthService authService,
			IMembershipRepository membershipRepository)
		{
			_bikeRepository = bikeRepository;
			_authService = authService;
			_membershipRepository = membershipRepository;

			_bikesChanged = (sender, args) =>
			{
				_bikes.Clear();

				foreach (var bike in _allBikes.Values
					.SelectMany(bikes => bikes)
					.Where(bike => bike.CurrentUser == null || bike.CurrentUser == _userId))
				{
					_bikes.Add(bike);
				}
			};
		}

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
			return _bikeRepository.Update(bike);
		}

		public Task ReturnBike(Bike bike)
		{
			bike.CurrentUser = null;
			return _bikeRepository.Update(bike);
		}
	}
}
