#nullable enable
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using CommunityBikeSharing.Models;
using CommunityBikeSharing.Services;
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

		private readonly NotifyCollectionChangedEventHandler _bikesChanged;
		private readonly NotifyCollectionChangedEventHandler _stationsChanged;

		public OverviewViewModel(
			IBikeService bikeService,
			ILocationService locationService,
			IStationService stationService,
			INavigationService navigationService,
			IDialogService dialogService)
		{
			_bikeService = bikeService;
			_locationService = locationService;
			_stationService = stationService;
			_navigationService = navigationService;
			_dialogService = dialogService;

			_bikesChanged = (sender, args) =>
			{
				OnPropertyChanged(nameof(BikesSorted));
				OnPropertyChanged(nameof(SummaryVisible));
				OnPropertyChanged(nameof(AllItems));
			};

			_stationsChanged = (sender, args) =>
			{
				OnPropertyChanged(nameof(SummaryVisible));
				OnPropertyChanged(nameof(AllItems));
			};

			ShowBikeOnMapCommand = new Command<Bike>(ShowBikeOnMap, CanShowBikeOnMap);
			LendBikeCommand = new Command<Bike>(LendBike, CanLendBike);
			ReturnBikeCommand = new Command<Bike>(ReturnBike, CanReturnBike);
			ReserveBikeCommand = new Command<Bike>(ReserveBike, CanReserveBike);
		}

		public IEnumerable<object> AllItems
		{
			get
			{
				if (AllStations == null)
				{
					return AllBikes?.Where(bike => bike.StationId == null) ?? Enumerable.Empty<object>();
				}

				return AllBikes == null ? AllStations
					: AllStations.AsEnumerable<object>().Concat(AllBikes.Where(bike => bike.StationId == null));
			}
		}

		private ObservableCollection<Station>? _allStations;
		[DisallowNull]
		public ObservableCollection<Station>? AllStations
		{
			get => _allStations;
			set
			{
				if (_allStations != null)
				{
					_allStations.CollectionChanged -= _stationsChanged;
				}

				value.CollectionChanged += _stationsChanged;

				_allStations = value;
				OnPropertyChanged();
				_stationsChanged.Invoke(null, null);
			}
		}

		private ObservableCollection<Bike>? _allBikes;
		[DisallowNull]
		public ObservableCollection<Bike>? AllBikes
		{
			get => _allBikes;
			set
			{
				if (_allBikes != null)
				{
					_allBikes.CollectionChanged -= _bikesChanged;
				}

				value.CollectionChanged += _bikesChanged;

				_allBikes = value;
				OnPropertyChanged();
				_bikesChanged.Invoke(null, null);
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
				OnPropertyChanged(nameof(BikesSorted));
			}
		}

		public IEnumerable<Bike> BikesSorted => AllBikes?
			.OrderBy(bike => bike.CurrentUser == null)
			.ThenBy(bike => bike.Location?.CalculateDistance(UserLocation, DistanceUnits.Kilometers))
			.ThenBy(bike => bike.Name) ?? Enumerable.Empty<Bike>();

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

		public string Heading => "Verfügbare Fahrräder:";
		public string Summary => "Zurzeit sind keine Fahrräder verfügbar. " +
		                         "Treten Sie einer Community bei, um Fahrräder auszuleihen.";

		public string ToggleMapText => ShowMap ? "Zur Listenansicht" : "Zur Kartenansicht";

		public Command<Bike> ShowBikeOnMapCommand { get; }
		public Command<Bike> LendBikeCommand { get; }
		public Command<Bike> ReturnBikeCommand { get; }
		public Command<Bike> ReserveBikeCommand { get; }

		public async void OnBikeSelected(Bike bike)
		{
			var actions = new (string, ICommand) []
			{
				("Auf Karte anzeigen", ShowBikeOnMapCommand),
				("Fahrrad ausleihen", LendBikeCommand),
				("Fahrrad zurückgeben", ReturnBikeCommand),
				("Fahrrad reservieren", ReserveBikeCommand)
			};

			await _dialogService.ShowActionSheet(bike.Name, "Abbrechen", actions, bike);
		}

		public bool SummaryVisible => AllStations?.Count == 0 &&  AllBikes?.Count == 0;

		public override async Task InitializeAsync()
		{
			AllBikes = _bikeService.GetAvailableBikes();

			AllStations = _stationService.GetAvailableStations();

			UserLocation = await _locationService.GetCurrentLocation();
			OnLocationChanged?.Invoke(UserLocation);
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
			UserLocation = await _locationService.GetCurrentLocation();
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
			//TODO: Open lock
			await _bikeService.LendBike(bike);
		}
		private bool CanLendBike(Bike bike) => string.IsNullOrEmpty(bike.CurrentUser);

		private async void ReturnBike(Bike bike)
		{
			// TODO: Close lock
			await _bikeService.ReturnBike(bike);
		}
		private bool CanReturnBike(Bike bike) => !string.IsNullOrEmpty(bike.CurrentUser);

		private void ReserveBike(Bike bike)
		{
			// TODO
		}
		private bool CanReserveBike(Bike bike) => CanLendBike(bike);

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
