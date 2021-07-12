using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using CommunityBikeSharing.Controls;
using CommunityBikeSharing.Models;
using CommunityBikeSharing.Services;
using CommunityBikeSharing.Services.Data.Bikes;
using CommunityBikeSharing.Services.Data.Stations;
using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.Essentials;

namespace CommunityBikeSharing.ViewModels
{
	public class OverviewViewModel : BaseViewModel
	{
		private readonly ILocationService _locationService;
		private readonly INavigationService _navigationService;
		private readonly IDialogService _dialogService;

		public OverviewViewModel(
			IBikeService bikeService,
			ILocationService locationService,
			IStationService stationService,
			INavigationService navigationService,
			IDialogService dialogService)
		{
			_locationService = locationService;
			_navigationService = navigationService;
			_dialogService = dialogService;
			_locationService = locationService;

			ShowBikeOnMapCommand = CreateCommand<Bike>(ShowBikeOnMap, CanShowBikeOnMap);
			RefreshUserLocationCommand = CreateCommand(RefreshUserLocation);
			ToggleMapCommand = CreateCommand(ToggleMap);
			
			ShowMap	= Preferences.Get("ShowMap", false);

			PropertyChanged += (_, args) =>
			{
				if (args.PropertyName == nameof(ShowMap)) 
				{
					OnPropertyChanged(nameof(ToggleMapText));
					Preferences.Set("ShowMap", ShowMap);
				}
			};
			
			var bikes = bikeService.GetAvailableBikes();
			bikes.CollectionChanged += (_, _) =>
			{
				AllBikes.ReplaceRange(bikes);

				OnPropertyChanged(nameof(MapItems));
				OnPropertyChanged(nameof(GroupedItems));
			};

			var stations = stationService.GetAvailableStations();
			stations.CollectionChanged += (_, _) =>
			{
				AllStations.ReplaceRange(stations);
				
				OnPropertyChanged(nameof(MapItems));
				OnPropertyChanged(nameof(GroupedItems));
			};
		}

		public override async Task InitializeAsync()
		{
			var location = await _locationService.GetCurrentLocation();

			if (location != null)
			{
				UserLocation = location;
			}
		}

		public ICommand ShowBikeOnMapCommand { get; }
		public ICommand RefreshUserLocationCommand { get; }
		public ICommand ToggleMapCommand { get; }
		
		public ObservableRangeCollection<Station> AllStations { get; } = new();
		public ObservableRangeCollection<Bike> AllBikes { get; } = new();

		private Location _userLocation = new();
		public Location UserLocation
		{
			get => _userLocation;
			set => SetProperty(ref _userLocation, value);
		}

		public IEnumerable<object> MapItems
			=> AllStations.Concat<object>(AllBikes.Where(bike => bike.StationId is null && bike.Location is not null));

		public List<ItemGroup> GroupedItems
		{
			get
			{
				var result = new List<ItemGroup>();

				if (AllStations.Count > 0)
				{
					result.Add(new ItemGroup("Stationen", AllStations));
				}

				var bikes = AllBikes.Where(bike => bike.StationId is null).ToList();

				if (bikes.Count > 0)
				{
					result.Add(new ItemGroup("Freie Fahrräder", bikes));
				}

				return result;
			}
		}

		private bool _showMap;
		public bool ShowMap
		{
			get => _showMap;
			set => SetProperty(ref _showMap, value);
		}

		public string Summary => "Zurzeit sind keine Fahrräder verfügbar. " +
		                         "Treten Sie einer Community bei, um Fahrräder auszuleihen.";

		public string ToggleMapText => ShowMap ? "Zur Listenansicht" : "Zur Kartenansicht";


		public async Task OnBikeSelected(Bike bike)
		{
			var bikeVM = App.GetViewModel<BikeViewModel>();
			
			var actions = new[]
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

		private bool _isRefreshing;
		public bool IsRefreshing
		{
			get => _isRefreshing;
			set => SetProperty(ref _isRefreshing, value);
		}

		private async Task RefreshUserLocation()
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
			if (bike.Location is null)
			{
				return;
			}

			UserLocation = bike.Location;

			ShowMap = true;
		}
		private bool CanShowBikeOnMap(Bike bike) => bike.Location != null && !ShowMap;

		private void ToggleMap() => ShowMap = !ShowMap;

		public Task OnStationSelected(Station station) =>
			_navigationService.NavigateTo<StationDetailViewModel>(
				StationDetailViewModel.NavigationParameters(station.CommunityId, station.Id));
	}
}
