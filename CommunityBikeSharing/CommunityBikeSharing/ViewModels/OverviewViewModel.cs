#nullable enable
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using CommunityBikeSharing.Controls;
using CommunityBikeSharing.Models;
using CommunityBikeSharing.Services;
using CommunityBikeSharing.Services.Data.Bikes;
using CommunityBikeSharing.Services.Data.Locks;
using CommunityBikeSharing.Services.Data.Stations;
using Microsoft.Extensions.DependencyInjection;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace CommunityBikeSharing.ViewModels
{
	public class OverviewViewModel : BaseViewModel
	{
		private readonly IBikeService _bikeService;
		private readonly ILocationService _locationService;
		private readonly IStationService _stationService;
		private readonly INavigationService _navigationService;
		private readonly IDialogService _dialogService;
		private readonly ILockService _lockService;

		public OverviewViewModel(
			IBikeService bikeService,
			ILocationService locationService,
			IStationService stationService,
			INavigationService navigationService,
			IDialogService dialogService,
			ILockService lockService)
		{
			_bikeService = bikeService;
			_locationService = locationService;
			_stationService = stationService;
			_navigationService = navigationService;
			_dialogService = dialogService;
			_locationService = locationService;
			_lockService = lockService;

			ShowBikeOnMapCommand = new Command<Bike>(ShowBikeOnMap, CanShowBikeOnMap);
			LendBikeCommand = new Command<Bike>(LendBike, bikeService.CanLendBike);
			ReturnBikeCommand = new Command<Bike>(ReturnBike, bikeService.CanReturnBike);
			ReserveBikeCommand = new Command<Bike>(ReserveBike, bikeService.CanReserveBike);
			DeleteReservationCommand = new Command<Bike>(DeleteReservation, bikeService.CanDeleteReservation);
			TakeBreakCommand = new Command<Bike>(TakeBreak, CanTakeBreak);
			EndBreakCommand = new Command<Bike>(EndBreak, CanEndBreak);
		}

		public IEnumerable<object> MapItems
		{
			get
			{
				return AllStations.Concat<object>(
					AllBikes.Where(bike => bike.StationId == null && bike.Location != null));
			}
		}

		public List<ItemGroup> GroupedItems
		{
			get
			{
				var result = new List<ItemGroup>();

				if (AllStations.Count > 0)
				{
					result.Add(new ItemGroup("Stationen", AllStations));
				}

				if (AllBikes.Any(bike => bike.StationId == null))
				{
					result.Add(new ItemGroup("Freie Fahrräder", AllBikes.Where(bike => bike.StationId == null)));
				}

				return result;
			}
		}

		private ObservableCollection<Station> _allStations = new ObservableCollection<Station>();
		public ObservableCollection<Station> AllStations
		{
			get => _allStations;
			set
			{
				_allStations.CollectionChanged -= OnStationsChanged;

				value.CollectionChanged += OnStationsChanged;

				_allStations = value;
				OnPropertyChanged();
				OnStationsChanged();

				void OnStationsChanged(object? sender = null, NotifyCollectionChangedEventArgs? e = null)
				{
					OnPropertyChanged(nameof(MapItems));
					OnPropertyChanged(nameof(GroupedItems));
				}
			}
		}


		private ObservableCollection<Bike> _allBikes = new ObservableCollection<Bike>();
		public ObservableCollection<Bike> AllBikes
		{
			get => _allBikes;
			set
			{
				_allBikes.CollectionChanged -= OnBikesChanged;

				value.CollectionChanged += OnBikesChanged;

				_allBikes = value;
				OnPropertyChanged();
				OnBikesChanged();

				void OnBikesChanged(object? sender = null, NotifyCollectionChangedEventArgs? e = null)
				{
					OnPropertyChanged(nameof(MapItems));
					OnPropertyChanged(nameof(GroupedItems));
				}
			}
		}

		private Location _userLocation = new Location();
		public Location UserLocation
		{
			get => _userLocation;
			set
			{
				_userLocation = value;
				OnPropertyChanged();
			}
		}

		private bool _showMap = Preferences.Get("ShowMap", false);
		public bool ShowMap
		{
			get => _showMap;
			set
			{
				_showMap = value;
				OnPropertyChanged();
				OnPropertyChanged(nameof(ToggleMapText));
				Preferences.Set("ShowMap", value);
			}
		}

		public delegate void LocationChanged(Location location);
		public event LocationChanged? OnLocationChanged;

		public string Summary => "Zurzeit sind keine Fahrräder verfügbar. " +
		                         "Treten Sie einer Community bei, um Fahrräder auszuleihen.";

		public string ToggleMapText => ShowMap ? "Zur Listenansicht" : "Zur Kartenansicht";

		public Command<Bike> ShowBikeOnMapCommand { get; }
		public Command<Bike> LendBikeCommand { get; }
		public Command<Bike> ReturnBikeCommand { get; }
		public Command<Bike> ReserveBikeCommand { get; }
		public Command<Bike> DeleteReservationCommand { get; }
		public Command<Bike> TakeBreakCommand { get; }
		public Command<Bike> EndBreakCommand { get; }

		public async void OnBikeSelected(Bike bike)
		{
			var actions = new (string, ICommand) []
			{
				("Auf Karte anzeigen", ShowBikeOnMapCommand),
				("Fahrrad ausleihen", LendBikeCommand),
				("Fahrrad zurückgeben", ReturnBikeCommand),
				("Fahrrad reservieren", ReserveBikeCommand),
				("Reservierung löschen", DeleteReservationCommand),
				("Pause machen", TakeBreakCommand),
				("Pause beenden", EndBreakCommand),
			};

			await _dialogService.ShowActionSheet(bike.Name, "Abbrechen", actions, bike);
		}

		public override async Task InitializeAsync()
		{
			AllBikes = _bikeService.GetAvailableBikes();

			AllStations = _stationService.GetAvailableStations();

			var location = await _locationService.GetCurrentLocation();

			if (location != null)
			{
				UserLocation = location;
				OnLocationChanged?.Invoke(UserLocation);
			}
		}

		private bool _isRefreshing;
		public bool IsRefreshing
		{
			get => _isRefreshing;
			set
			{
				_isRefreshing = value;
				OnPropertyChanged();
			}
		}
		public ICommand RefreshUserLocationCommand => new Command(RefreshUserLocation);
		private async void RefreshUserLocation()
		{
			IsRefreshing = true;

			var location = await _locationService.GetCurrentLocation();

			if (location != null)
			{
				UserLocation = location;
				OnLocationChanged?.Invoke(UserLocation);
			}

			IsRefreshing = false;
		}

		private void ShowBikeOnMap(Bike bike)
		{
			if (bike.Location == null)
			{
				return;
			}

			ShowMap = true;
			OnLocationChanged?.Invoke(bike.Location);
		}
		private bool CanShowBikeOnMap(Bike bike) => bike.Location != null && !ShowMap;

		private async void LendBike(Bike bike)
		{
			await _bikeService.LendBike(bike);
		}

		private async void ReturnBike(Bike bike)
		{
			await _bikeService.ReturnBike(bike);
		}

		private async void ReserveBike(Bike bike)
		{
			await _bikeService.ReserveBike(bike);

			var dateTime = bike.ReservedUntil!.Value.ToLocalTime();

			string formattedDateTime = dateTime.ToString(dateTime.Date == DateTime.Now.Date ? "HH:mm" : "dd.MM, hh:mm");

			await _dialogService.ShowMessage("Fahrrad reserviert",
				$"Das Fahrrad wurde bis {formattedDateTime} reserviert. " +
				"Es kann bis zu diesem Zeitpunkt nur von Ihnen ausgeliehen werden.");
		}

		private async void DeleteReservation(Bike bike)
		{
			await _bikeService.DeleteReservation(bike);
		}

		private async void TakeBreak(Bike bike)
		{
			await _lockService.CloseLock(bike);
		}

		private bool CanTakeBreak(Bike bike) => bike.Lent && bike.HasLock && bike.LockState != Lock.State.Closed;

		private async void EndBreak(Bike bike)
		{
			await _lockService.OpenLock(bike);
		}

		private bool CanEndBreak(Bike bike) => bike.Lent && bike.HasLock && bike.LockState != Lock.State.Open;

		public ICommand ToggleMapCommand => new Command(ToggleMap);

		private void ToggleMap()
		{
			ShowMap = !ShowMap;
		}

		public async void OnStationSelected(Station station)
		{
			await _navigationService.NavigateTo<StationDetailViewModel>(
				StationDetailViewModel.NavigationParameters(station.CommunityId, station.Id));
		}
	}
}
