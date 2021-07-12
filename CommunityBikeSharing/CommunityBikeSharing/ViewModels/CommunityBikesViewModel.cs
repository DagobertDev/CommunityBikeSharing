using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using CommunityBikeSharing.Models;
using CommunityBikeSharing.Services;
using CommunityBikeSharing.Services.Data.Bikes;
using CommunityBikeSharing.Services.Data.Memberships;

namespace CommunityBikeSharing.ViewModels
{
	public class CommunityBikesViewModel : BaseViewModel
	{
		private readonly IDialogService _dialogService;
		private readonly IBikeService _bikeService;
		private readonly INavigationService _navigationService;
		private readonly string _communityId;

		public CommunityBikesViewModel(IDialogService dialogService,
			IBikeService bikeService,
			IMembershipService membershipService,
			INavigationService navigationService,
			string id)
		{
			_dialogService = dialogService;
			_bikeService = bikeService;
			_navigationService = navigationService;

			_communityId = id;
			
			AddBikeCommand = CreateCommand(AddBike);
			EditBikeCommand = CreateCommand<Bike>(EditBike, CanEditBike);

			PropertyChanged += (_, args) =>
			{
				if (args.PropertyName == nameof(CurrentUserMembership))
				{
					OnPropertyChanged(nameof(CanAddBike));
				}
			};
			
			Bikes = _bikeService.ObserveBikesFromCommunity(_communityId);
			Bikes.CollectionChanged += (_, _) => OnPropertyChanged(nameof(SortedBikes));

			membershipService.Observe(_communityId).Subscribe(
				membership => CurrentUserMembership = membership,
				exception => CurrentUserMembership = null);
		}
		
		public ICommand AddBikeCommand { get; }
		public ICommand EditBikeCommand { get; }
		
		public ObservableCollection<Bike> Bikes { get; }
		public IEnumerable<Bike> SortedBikes => Bikes.OrderBy(bike => bike.Name);


		private CommunityMembership? _currentUserMembership;
		private CommunityMembership? CurrentUserMembership
		{
			get => _currentUserMembership;
			set => SetProperty(ref _currentUserMembership, value);
		}

		public bool CanAddBike => CurrentUserMembership is {IsCommunityAdmin: true};

		private async Task AddBike()
		{
			var name = await _dialogService.ShowTextEditor("Name eingeben",
				"Bitte geben Sie den Namen des Fahrrads ein.");

			if (string.IsNullOrEmpty(name))
			{
				return;
			}

			await _bikeService.Add(name, _communityId);
		}

		private Task EditBike(Bike bike) => _navigationService.NavigateTo<EditBikeViewModel>(bike.CommunityId, bike.Id);
		private bool CanEditBike(Bike bike) => CurrentUserMembership is {IsCommunityAdmin: true};
	}
}
