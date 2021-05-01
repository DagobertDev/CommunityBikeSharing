using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using CommunityBikeSharing.Models;
using CommunityBikeSharing.Services;
using Xamarin.Forms;

namespace CommunityBikeSharing.ViewModels
{
	public class CommunityBikesViewModel : BaseViewModel
	{
		private readonly IDialogService _dialogService;
		private readonly IBikeRepository _bikeRepository;
		private readonly IUserService _userService;
		private readonly IMembershipRepository _membershipRepository;
		private readonly string _communityId;

		public CommunityBikesViewModel(IDialogService dialogService,
			IBikeRepository bikeRepository,
			IUserService userService,
			IMembershipRepository membershipRepository,
			string id)
		{
			_dialogService = dialogService;
			_bikeRepository = bikeRepository;
			_userService = userService;
			_membershipRepository = membershipRepository;

			_communityId = id;

			_bikesChanged = (sender, args) => OnPropertyChanged(nameof(SortedBikes));
		}

		private readonly NotifyCollectionChangedEventHandler _bikesChanged;

		private ObservableCollection<Bike> _bikes;

		public ObservableCollection<Bike> Bikes
		{
			get => _bikes;
			set
			{
				if (_bikes != null)
				{
					_bikes.CollectionChanged -= _bikesChanged;
				}

				_bikes = value;
				_bikes.CollectionChanged += _bikesChanged;

				OnPropertyChanged();
				_bikesChanged(null, null);
			}
		}

		private CommunityMembership _currentUserMembership;
		private CommunityMembership CurrentUserMembership
		{
			get => _currentUserMembership;
			set
			{
				_currentUserMembership = value;
				OnPropertyChanged();
				OnPropertyChanged(nameof(CanAddBike));
			}
		}

		public IEnumerable<Bike> SortedBikes => Bikes?.OrderBy(m => m.Name);

		public ICommand AddBikeCommand => new Command(AddBike);
		public bool CanAddBike => CurrentUserMembership is {IsCommunityAdmin: true};

		private async void AddBike()
		{
			var name = await _dialogService.ShowTextEditor("Name eingeben",
				"Bitte geben Sie den Namen des Fahrrads ein.");

			await _bikeRepository.Add(name, _communityId);
		}

		public override async Task InitializeAsync()
		{
			Bikes = _bikeRepository.ObserveBikesFromCommunity(_communityId);

			var members = _membershipRepository.ObserveMembershipsFromCommunity(_communityId);

			var user = await _userService.GetCurrentUser();
			CurrentUserMembership = members.Single(m => m.UserId == user.Id);
		}

		public ICommand EditBikeCommand => new Command<Bike>(EditBike);

		private void EditBike(Bike bike)
		{
			var actions = new []
			{
				("Fahrrad umbenennen", RenameBikeCommand),
				("Fahrrad entfernen", DeleteBikeCommand)
			};

			_dialogService.ShowActionSheet(bike.Name, "Abbrechen", actions, bike);
		}

		public ICommand RenameBikeCommand => new Command<Bike>(RenameBike, CanRenameBike);

		private async void RenameBike(Bike bike)
		{
			var name = await _dialogService.ShowTextEditor(
				"Fahrrad umbenennen", "Bitte geben Sie den neuen Namen ein:");

			if (string.IsNullOrEmpty(name))
			{
				return;
			}

			bike.Name = name;
			await _bikeRepository.Update(bike);
		}

		private bool CanRenameBike(Bike bike) => CurrentUserMembership is {IsCommunityAdmin: true};

		public ICommand DeleteBikeCommand => new Command<Bike>(DeleteBike, CanDeleteBike);

		private async void DeleteBike(Bike bike)
		{
			var confirmed = await _dialogService.ShowConfirmation(
				"Fahrrad entfernen", $"Wollen Sie das Fahrrad \"{bike.Name}\" wirklich entfernen?");

			if (!confirmed)
			{
				return;
			}

			await _bikeRepository.Delete(bike);
		}

		private bool CanDeleteBike(Bike bike) => CurrentUserMembership is {IsCommunityAdmin: true};
	}
}
