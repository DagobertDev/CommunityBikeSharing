using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using CommunityBikeSharing.Models;
using CommunityBikeSharing.Services;
using CommunityBikeSharing.Services.Data.Bikes;
using CommunityBikeSharing.Services.Data.Memberships;
using Xamarin.Forms;

namespace CommunityBikeSharing.ViewModels
{
	public class CommunityBikesViewModel : BaseViewModel
	{
		private readonly IDialogService _dialogService;
		private readonly IBikeService _bikeService;
		private readonly IMembershipService _membershipService;
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
			_membershipService = membershipService;
			_navigationService = navigationService;

			_communityId = id;

			_bikesChanged = (_, _) => OnPropertyChanged(nameof(SortedBikes));
		}

		private readonly NotifyCollectionChangedEventHandler _bikesChanged;

		private ObservableCollection<Bike>? _bikes;

		[DisallowNull]
		public ObservableCollection<Bike>? Bikes
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

		private CommunityMembership? _currentUserMembership;
		private CommunityMembership? CurrentUserMembership
		{
			get => _currentUserMembership;
			set
			{
				_currentUserMembership = value;
				OnPropertyChanged();
				OnPropertyChanged(nameof(CanAddBike));
			}
		}

		public IEnumerable<Bike> SortedBikes => Bikes?.OrderBy(m => m.Name) ?? Enumerable.Empty<Bike>();

		public ICommand AddBikeCommand => new Command(AddBike);
		public bool CanAddBike => CurrentUserMembership is {IsCommunityAdmin: true};

		private async void AddBike()
		{
			var name = await _dialogService.ShowTextEditor("Name eingeben",
				"Bitte geben Sie den Namen des Fahrrads ein.");

			if (string.IsNullOrEmpty(name))
			{
				return;
			}

			await _bikeService.Add(name, _communityId);
		}

		public override Task InitializeAsync()
		{
			Bikes = _bikeService.ObserveBikesFromCommunity(_communityId);

			_membershipService.Observe(_communityId).Subscribe(
				membership => CurrentUserMembership = membership,
				exception => CurrentUserMembership = null);

			return Task.CompletedTask;
		}

		public ICommand EditBikeCommand => new Command<Bike>(async bike => await EditBike(bike));

		private Task EditBike(Bike bike) => _navigationService.NavigateTo<EditBikeViewModel>(bike.CommunityId, bike.Id);
	}
}
