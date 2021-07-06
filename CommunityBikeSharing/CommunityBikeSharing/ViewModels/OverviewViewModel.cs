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
using CommunityBikeSharing.Services.Data.Stations;
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
			_locationService = locationService;

			ShowBikeOnMapCommand = new Command<Bike>(ShowBikeOnMap, CanShowBikeOnMap);
		}

		private Location _userLocation = new Location();

		public Location UserLocation
		{
			get => _userLocation;
			set
			{
				_userLocation = value;
				OnLocationChanged?.Invoke(this, value);
				OnPropertyChanged();
			}
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

		public event EventHandler<Location>? OnLocationChanged;

		public string Summary => "Zurzeit sind keine Fahrräder verfügbar. " +
		                         "Treten Sie einer Community bei, um Fahrräder auszuleihen.";

		public string ToggleMapText => ShowMap ? "Zur Listenansicht" : "Zur Kartenansicht";

		public Command<Bike> ShowBikeOnMapCommand { get; }

		public async void OnBikeSelected(Bike bike)
		{
			var bikeVM = App.GetViewModel<BikeViewModel>();
			
			var actions = new (string, ICommand) []
			{
				("Auf Karte anzeigen", ShowBikeOnMapCommand),
				("Fahrrad ausleihen", bikeVM.LendBikeCommand),
				("Fahrrad zurückgeben", bikeVM.ReturnBikeCommand),
				("Fahrrad reservieren", bikeVM.ReserveBikeCommand),
				("Reservierung löschen", bikeVM.DeleteReservationCommand),
				("Pause machen (Schloss schließen)", bikeVM.TakeBreakCommand),
				("Pause beenden (Schloss öffnen)", bikeVM.EndBreakCommand),
				("Problem melden", bikeVM.ReportProblemCommand),
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
			}

			IsRefreshing = false;
		}

		private void ShowBikeOnMap(Bike bike)
		{
			if (bike.Location == null)
			{
				return;
			}

			UserLocation = bike.Location;

			ShowMap = true;
		}
		private bool CanShowBikeOnMap(Bike bike) => bike.Location != null && !ShowMap;

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
