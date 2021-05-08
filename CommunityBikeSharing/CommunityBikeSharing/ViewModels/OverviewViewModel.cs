using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks;
using CommunityBikeSharing.Models;
using CommunityBikeSharing.Services;
using CommunityBikeSharing.Services.Data;

namespace CommunityBikeSharing.ViewModels
{
	public class OverviewViewModel : BaseViewModel
	{
		private string _userId;
		private readonly IBikeRepository _bikeRepository;
		private readonly IMembershipRepository _membershipRepository;
		private readonly IAuthService _authService;

		private readonly NotifyCollectionChangedEventHandler _bikesChanged;

		private readonly IDictionary<string, ObservableCollection<Bike>> _allBikes =
			new Dictionary<string, ObservableCollection<Bike>>();

		public IEnumerable<Bike> BikesSorted => _allBikes.Values
			.SelectMany(bikes => bikes)
			.OrderBy(bike => bike.Name);

		public OverviewViewModel(IBikeRepository bikeRepository,
			IMembershipRepository membershipRepository,
			IAuthService authService)
		{
			_bikeRepository = bikeRepository;
			_membershipRepository = membershipRepository;
			_authService = authService;

			_bikesChanged = (sender, args) =>
			{
				OnPropertyChanged(nameof(BikesSorted));
				OnPropertyChanged(nameof(SummaryVisible));
			};
		}

		public string Heading => "Verfügbare Fahrräder:";
		public string Summary => "Zurzeit sind keine Fahrräder verfügbar. " +
		                         "Treten Sie einer Community bei, um Fahrräder auszuleihen.";
		public bool SummaryVisible => !BikesSorted.Any();

		public override Task InitializeAsync()
		{
			var newUserId = _authService.GetCurrentUserId();

			if (_userId == newUserId)
			{
				return Task.CompletedTask;
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

				OnPropertyChanged(nameof(BikesSorted));
				OnPropertyChanged(nameof(SummaryVisible));
			};

			return Task.CompletedTask;
		}
	}
}
