using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
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

		private readonly NotifyCollectionChangedEventHandler _bikesChanged;

		private ObservableCollection<Bike> _allBikes;
		public ObservableCollection<Bike> AllBikes
		{
			get => _allBikes;
			set
			{
				if (_allBikes != null)
				{
					_allBikes.CollectionChanged -= _bikesChanged;
				}

				if (value != null)
				{
					value.CollectionChanged += _bikesChanged;
				}

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
			.ThenBy(bike => bike.Location.CalculateDistance(UserLocation, DistanceUnits.Kilometers))
			.ThenBy(bike => bike.Name) ?? Enumerable.Empty<Bike>();

		public OverviewViewModel(IBikeService bikeService, ILocationService locationService)
		{
			_bikeService = bikeService;
			_locationService = locationService;

			_bikesChanged = (sender, args) =>
			{
				OnPropertyChanged(nameof(BikesSorted));
				OnPropertyChanged(nameof(SummaryVisible));
			};
		}

		public string Heading => "Verfügbare Fahrräder:";
		public string Summary => "Zurzeit sind keine Fahrräder verfügbar. " +
		                         "Treten Sie einer Community bei, um Fahrräder auszuleihen.";

		private Bike _selectedBike;
		public Bike SelectedBike
		{
			get => _selectedBike;
			set
			{
				_selectedBike = value;
				OnPropertyChanged();
				OnPropertyChanged(nameof(SelectedBikeName));
				OnPropertyChanged(nameof(HasSelectedBike));
				OnPropertyChanged(nameof(CanShowBikeOnMap));
				OnPropertyChanged(nameof(CanLendBike));
				OnPropertyChanged(nameof(CanReturnBike));
				OnPropertyChanged(nameof(CanReserveBike));
			}
		}

		public bool HasSelectedBike => SelectedBike != null;
		public string SelectedBikeName => SelectedBike?.Name;

		public bool SummaryVisible => AllBikes?.Count == 0;

		public override async Task InitializeAsync()
		{
			AllBikes = _bikeService.GetAvailableBikes();

			UserLocation = await _locationService.GetCurrentLocation();
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

		public ICommand ShowBikeOnMapCommand => new Command<Bike>(ShowBikeOnMap);
		private async void ShowBikeOnMap(Bike bike)
		{
			await Map.OpenAsync(bike.Location);
		}

		public bool CanShowBikeOnMap => SelectedBike?.Location != null;

		public ICommand LendBikeCommand => new Command<Bike>(LendBike);
		private async void LendBike(Bike bike)
		{
			//TODO: Open lock
			await _bikeService.LendBike(bike);
			SelectedBike = null;
		}
		public bool CanLendBike => string.IsNullOrEmpty(SelectedBike.CurrentUser);
		public ICommand ReturnBikeCommand => new Command<Bike>(ReturnBike);
		public bool CanReturnBike => !string.IsNullOrEmpty(SelectedBike.CurrentUser);

		private async void ReturnBike(Bike bike)
		{
			// TODO: Close lock
			await _bikeService.ReturnBike(bike);
			SelectedBike = null;
		}
		public bool CanReserveBike => CanLendBike;
	}
}
