using System;
using System.Threading.Tasks;
using System.Windows.Input;
using CommunityBikeSharing.Models;
using CommunityBikeSharing.Services;
using CommunityBikeSharing.Services.Data.Bikes;
using CommunityBikeSharing.Services.Data.Communities;
using CommunityBikeSharing.Services.Data.Locks;
using CommunityBikeSharing.Services.Data.Memberships;
using Newtonsoft.Json;

namespace CommunityBikeSharing.ViewModels
{
	public class EditBikeViewModel : BaseViewModel
	{
		private readonly IBikeService _bikeService;
		private readonly ILockService _lockService;
		private readonly IMembershipService _membershipService;
		private readonly ICommunityService _communityService;
		private readonly IDialogService _dialogService;
		private readonly ILocationPicker _locationPicker;
		private readonly IQRCodeScanner _qrCodeScanner;
		private readonly INavigationService _navigationService;

		private Community? Community { get; set; }
		
		private readonly string _communityId;
		private readonly string _bikeId;
		
		public EditBikeViewModel(
			IDialogService dialogService,
			ILockService lockService,
			IBikeService bikeService,
			IMembershipService membershipService,
			ICommunityService communityService,
			ILocationPicker locationPicker,
			IQRCodeScanner qrCodeScanner,
			INavigationService navigationService,
			string communityId, 
			string bikeId)
		{
			_dialogService = dialogService;
			_lockService = lockService;
			_bikeService = bikeService;
			_membershipService = membershipService;
			_communityService = communityService;
			_locationPicker = locationPicker;
			_qrCodeScanner = qrCodeScanner;
			_navigationService = navigationService;
			
			_communityId = communityId;
			_bikeId = bikeId;
			
			SetLocationCommand = CreateCommand<Bike>(SetLocation, CanSetLocation);
			RenameBikeCommand = CreateCommand<Bike>(RenameBike, CanRenameBike); 
			DeleteBikeCommand = CreateCommand<Bike>(DeleteBike, CanDeleteBike); 
			ShowCurrentLenderCommand = CreateCommand<Bike>(ShowCurrentLender, CanShowCurrentLender); 
			AddLockCommand = CreateCommand<Bike>(AddLock, CanAddLock); 
			AddLockWithQRCodeCommand = CreateCommand<Bike>(AddLockWithQRCode, CanAddLock); 
			OpenLockCommand = CreateCommand<Bike>(OpenLock, CanOpenLock); 
			CloseLockCommand = CreateCommand<Bike>(CloseLock, CanCloseLock); 
			RemoveLockCommand = CreateCommand<Bike>(RemoveLock, CanRemoveLock);

			PropertyChanged += (_, args) =>
			{
				switch (args.PropertyName)
				{
					case nameof(Bike):
					{
						OnPropertyChanged(nameof(Name));
						OnPropertyChanged(nameof(LendState));
						OnPropertyChanged(nameof(LockState));
						break;
					}

					case nameof(CurrentUserMembership):
					{
						// Needed to update permissions for the commands
						OnPropertyChanged(nameof(Bike));
						break;
					}
				}
			};
		}

		public override Task InitializeAsync()
		{
			_membershipService.Observe(_communityId).Subscribe(
				membership => CurrentUserMembership = membership,
				exception => CurrentUserMembership = null);
			
			_communityService.Observe(_communityId).Subscribe(
				community => Community = community,
				exception => Community = null);

			_bikeService.Observe(_communityId, _bikeId).Subscribe(bike => Bike = bike);
			
			return Task.CompletedTask;
		}
		
		private Bike? _bike;
		public Bike? Bike
		{
			get => _bike;
			set => SetProperty(ref _bike, value);
		}
		
		private CommunityMembership? _currentUserMembership;
		private CommunityMembership? CurrentUserMembership
		{
			get => _currentUserMembership;
			set => SetProperty(ref _currentUserMembership, value);
		}

		public string Name => Bike?.Name ?? string.Empty;

		public string LendState => 
			Bike switch 
			{
				null => string.Empty, 
				{Lent: true} => "Ausgeliehen", 
				{Reserved: true} => "Reserviert", 
				{} => "Verfügbar"
			};

		public string LockState =>
			Bike?.LockState switch
			{
				Lock.State.None => "Kein Schloss vorhanden",
				Lock.State.Open => "Geöffnet",
				Lock.State.Closed => "Geschlossen",
				_ => "Unbekannt"
			};

		public ICommand SetLocationCommand { get; }
		public ICommand RenameBikeCommand { get; }
		public ICommand DeleteBikeCommand { get; }
		public ICommand ShowCurrentLenderCommand { get; } 
		public ICommand AddLockCommand { get; }
		public ICommand AddLockWithQRCodeCommand { get; }
		public ICommand OpenLockCommand { get; }
		public ICommand CloseLockCommand { get; }
		public ICommand RemoveLockCommand { get; }

		private async Task RenameBike(Bike bike)
		{
			var name = await _dialogService.ShowTextEditor(
				"Fahrrad umbenennen", "Bitte geben Sie den neuen Namen ein:");

			if (string.IsNullOrEmpty(name))
			{
				return;
			}

			await _bikeService.Rename(bike, name);
		}

		private bool CanRenameBike(Bike? bike) => IsCommunityAdmin;
		
		private async Task DeleteBike(Bike bike)
		{
			var confirmed = await _dialogService.ShowConfirmation(
				"Fahrrad entfernen", $"Möchten Sie das Fahrrad \"{bike.Name}\" wirklich löschen?");

			if (!confirmed)
			{
				return;
			}

			await _bikeService.Delete(bike);
			await _navigationService.NavigateBack();
		}

		private bool CanDeleteBike(Bike? bike) => IsCommunityAdmin;

		private async Task SetLocation(Bike bike)
		{
			var location = await _locationPicker.PickLocation();

			if (location == null)
			{
				return;
			}
			
			await _bikeService.UpdateLocation(bike, location);
			await _dialogService.ShowMessage("Standort aktualisiert", "Der Standort des Fahrrads wurde aktualisiert");
		}

		private bool CanSetLocation(Bike? bike) => IsCommunityAdmin && bike is {Lent: false};
		
		private async Task ShowCurrentLender(Bike bike)
		{
			var lenderId = bike.CurrentUser;

			var membership = await _membershipService.Get(_communityId, lenderId!);

			await _dialogService.ShowMessage("Aktueller Ausleiher", $"Das Fahrrad wird aktuell von {membership.Name} " 
			                                                        + $"{(bike.Lent ? "ausgeliehen" : "reserviert")}.");
		}

		private bool CanShowCurrentLender(Bike? bike) => IsCommunityAdmin 
		                                                 && Community is {ShowCurrentUser: true}
		                                                 && bike != null
		                                                 && (bike.Lent || bike.Reserved);

		private async Task AddLock(Bike bike)
		{
			var name = await _dialogService.ShowTextEditor("Name eingeben",
				"Bitte geben Sie den Namen des Schlosses ein");

			if (string.IsNullOrEmpty(name))
			{
				return;
			}

			var key = await _dialogService.ShowTextEditor("Schlüssel eingeben",
				"Bitte geben Sie den Schlüssel des Schlosses ein");
			
			if (string.IsNullOrEmpty(key))
			{
				return;
			}


			await _lockService.Add(bike, name, key);
		}

		private bool CanAddLock(Bike? bike) => IsCommunityAdmin && bike is {HasLock: false};
		
		private async Task AddLockWithQRCode(Bike bike)
		{
			var result = await _qrCodeScanner.Scan();

			if (result == null)
			{
				return;
			}
			
			var @lock = new { name = string.Empty, token = string.Empty };

			@lock = JsonConvert.DeserializeAnonymousType(result, @lock);

			if (@lock == null)
			{
				return;
			}

			await _lockService.Add(bike, @lock.name, @lock.token);
			await _dialogService.ShowMessage("Schloss hinzugefügt",
				$"Das Schloss \"{@lock.name}\" wurde erfolgreich mit dem Fahrrad \"{bike.Name}\" verbunden.");
		}
		
		private async Task OpenLock(Bike bike)
		{
			await _lockService.OpenLock(bike);
		}

		private bool CanOpenLock(Bike? bike) => IsCommunityAdmin 
		                                        && bike is {HasLock: true, LockState: not Lock.State.Open};


		private async Task CloseLock(Bike bike)
		{
			await _lockService.CloseLock(bike);
		}

		private bool CanCloseLock(Bike? bike) => IsCommunityAdmin 
		                                         && bike is { HasLock: true, LockState: not Lock.State.Closed };
		
		private async Task RemoveLock(Bike bike)
		{
			var confirmed =  await _dialogService.ShowConfirmation("Schloss entfernen", "Möchten Sie das Schloss wirklich entfernen?");

			if (!confirmed)
			{
				return;
			}

			await _lockService.Remove(bike);
		}

		private bool CanRemoveLock(Bike? bike) => IsCommunityAdmin 
		                                          && bike is {HasLock: true};

		private bool IsCommunityAdmin => CurrentUserMembership is {IsCommunityAdmin: true};
	}
}
