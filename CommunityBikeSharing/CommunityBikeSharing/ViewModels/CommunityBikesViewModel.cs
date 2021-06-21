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
using CommunityBikeSharing.Services.Data.Communities;
using CommunityBikeSharing.Services.Data.Locks;
using CommunityBikeSharing.Services.Data.Memberships;
using Xamarin.Forms;

namespace CommunityBikeSharing.ViewModels
{
	public class CommunityBikesViewModel : BaseViewModel
	{
		private readonly IDialogService _dialogService;
		private readonly IBikeService _bikeService;
		private readonly IMembershipService _membershipService;
		private readonly ICommunityService _communityService;
		private readonly ILockService _lockService;
		private readonly string _communityId;

		public CommunityBikesViewModel(IDialogService dialogService,
			IBikeService bikeService,
			IMembershipService membershipService,
			ICommunityService communityService,
			ILockService lockService,
			string id)
		{
			_dialogService = dialogService;
			_bikeService = bikeService;
			_membershipService = membershipService;
			_communityService = communityService;
			_lockService = lockService;

			_communityId = id;

			_bikesChanged = (sender, args) => OnPropertyChanged(nameof(SortedBikes));
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

		private Community? _community;
		private Community? Community
		{
			get => _community;
			set
			{
				_community = value;
				OnPropertyChanged();
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

			_communityService.Observe(_communityId).Subscribe(
				community => Community = community,
				exception => Community = null);

			return Task.CompletedTask;
		}

		public ICommand EditBikeCommand => new Command<Bike>(EditBike);

		private void EditBike(Bike bike)
		{
			var actions = new []
			{
				("Aktuellen Ausleiher anzeigen", ShowCurrentLenderCommand),
				("Fahrrad umbenennen", RenameBikeCommand),
				("Fahrrad entfernen", DeleteBikeCommand),
				("Schloss hinzufügen", AddLockCommand),
				("Schloss entfernen", RemoveLockCommand),
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

			await _bikeService.Rename(bike, name);
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

			await _bikeService.Delete(bike);
		}

		private bool CanDeleteBike(Bike bike) => CurrentUserMembership is {IsCommunityAdmin: true};

		public ICommand ShowCurrentLenderCommand => new Command<Bike>(ShowCurrentLender, CanShowCurrentLender);

		private async void ShowCurrentLender(Bike bike)
		{
			var lenderId = bike.CurrentUser;

			var membership = await _membershipService.Get(_communityId, lenderId!);

			await _dialogService.ShowMessage("", $"{membership.Name}");
		}

		private bool CanShowCurrentLender(Bike bike) => Community is {ShowCurrentUser: true}
		                                                && (bike.Lent || bike.Reserved)
		                                                && CurrentUserMembership is {IsCommunityAdmin: true};

		public ICommand AddLockCommand => new Command<Bike>(AddLock, CanAddLock);

		private async void AddLock(Bike bike)
		{
			var name = await _dialogService.ShowTextEditor("Name eingeben",
				"Bitte geben Sie den Namen des Schlosses ein");

			if (string.IsNullOrEmpty(name))
			{
				return;
			}

			var key = await _dialogService.ShowTextEditor("Schlüssel eingeben",
				"Bitte geben Sie den Schlüssel des Schlosses ein");

			await _lockService.Add(bike, name, key);
		}

		private bool CanAddLock(Bike bike) => !bike.HasLock &&
		                                      CurrentUserMembership is {IsCommunityAdmin: true};

		public ICommand RemoveLockCommand => new Command<Bike>(RemoveLock, CanRemoveLock);

		private async void RemoveLock(Bike bike)
		{
			var confirmed =  await _dialogService.ShowConfirmation("Schloss entfernen", "Möchten Sie das Schloss wirklich entfernen?");

			if (!confirmed)
			{
				return;
			}

			await _lockService.Remove(bike);
		}

		private bool CanRemoveLock(Bike bike) => bike.HasLock &&
		                                         CurrentUserMembership is {IsCommunityAdmin: true};
	}
}
