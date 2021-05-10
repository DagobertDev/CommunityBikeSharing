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
	public class OverviewViewModel : BaseViewModel
	{
		private readonly IBikeService _bikeService;

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

		public IEnumerable<Bike> BikesSorted => AllBikes?
			.OrderBy(bike => bike.CurrentUser == null)
			.ThenBy(bike => bike.Name);

		public OverviewViewModel(IBikeService bikeService)
		{
			_bikeService = bikeService;

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
				OnPropertyChanged(nameof(CanLendBike));
				OnPropertyChanged(nameof(CanReturnBike));
				OnPropertyChanged(nameof(CanReserveBike));
			}
		}

		public bool HasSelectedBike => SelectedBike != null;
		public string SelectedBikeName => SelectedBike?.Name;

		public bool SummaryVisible => AllBikes?.Count == 0;

		public override Task InitializeAsync()
		{
			AllBikes = _bikeService.GetAvailableBikes();

			return Task.CompletedTask;
		}

		public ICommand LendBikeCommand => new Command<Bike>(LendBike);
		public async void LendBike(Bike bike)
		{
			//TODO: Open lock
			await _bikeService.LendBike(bike);
			SelectedBike = null;
		}
		public bool CanLendBike => string.IsNullOrEmpty(SelectedBike.CurrentUser);
		public ICommand ReturnBikeCommand => new Command<Bike>(ReturnBike);
		public bool CanReturnBike => !string.IsNullOrEmpty(SelectedBike.CurrentUser);

		public async void ReturnBike(Bike bike)
		{
			// TODO: Close lock
			await _bikeService.ReturnBike(bike);
			SelectedBike = null;
		}
		public bool CanReserveBike => CanLendBike;
	}
}
