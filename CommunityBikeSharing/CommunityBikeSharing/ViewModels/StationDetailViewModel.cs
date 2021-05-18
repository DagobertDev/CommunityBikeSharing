#nullable enable
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using CommunityBikeSharing.Models;
using CommunityBikeSharing.Services;
using CommunityBikeSharing.Services.Data.Bikes;
using CommunityBikeSharing.Services.Data.Stations;
using Xamarin.Forms;

namespace CommunityBikeSharing.ViewModels
{
	public class StationDetailViewModel : BaseViewModel
	{
		public static string[] NavigationParameters(string communityId, string stationId) =>
			new [] {communityId, stationId};

		private readonly IStationService _stationService;
		private readonly IBikeService _bikeService;
		private readonly IDialogService _dialogService;

		private readonly string _communityId;
		private readonly string _stationId;

		public StationDetailViewModel(
			IStationService stationService,
			IBikeService bikeService,
			IDialogService dialogService,
			string communityId,
			string stationId)
		{
			_stationService = stationService;
			_bikeService = bikeService;
			_dialogService = dialogService;
			_communityId = communityId;
			_stationId = stationId;

			LendBikeCommand = new Command<Bike>(LendBike, CanLendBike);
			ReturnBikeCommand = new Command<Bike>(ReturnBike, CanReturnBike);
			ReserveBikeCommand = new Command<Bike>(ReserveBike, CanReserveBike);
		}

		private Station? _station;
		private Station? Station
		{
			get => _station;
			set
			{
				_station = value;
				OnPropertyChanged();
				OnPropertyChanged(nameof(Name));
			}
		}

		private ObservableCollection<Bike> _bikes = new ObservableCollection<Bike>();
		public ObservableCollection<Bike> Bikes
		{
			get => _bikes;
			set
			{
				_bikes = value;
				OnPropertyChanged();
			}
		}

		public override async Task InitializeAsync()
		{
			Station = await _stationService.Get(_communityId, _stationId);
			Bikes = _bikeService.ObserveBikesFromStation(Station);
		}

		public string Name => Station?.Name ?? string.Empty;

		public Command<Bike> LendBikeCommand { get; }
		public Command<Bike> ReturnBikeCommand { get; }
		public Command<Bike> ReserveBikeCommand { get; }

		public async void OnBikeSelected(Bike bike)
		{
			var actions = new (string, ICommand) []
			{
				("Fahrrad ausleihen", LendBikeCommand),
				("Fahrrad zurückgeben", ReturnBikeCommand),
				("Fahrrad reservieren", ReserveBikeCommand)
			};

			await _dialogService.ShowActionSheet(bike.Name, "Abbrechen", actions, bike);
		}

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
	}
}
