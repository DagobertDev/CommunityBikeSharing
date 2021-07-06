using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using CommunityBikeSharing.Models;
using CommunityBikeSharing.Services;
using CommunityBikeSharing.Services.Data.Bikes;
using CommunityBikeSharing.Services.Data.Stations;

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
				OnPropertyChanged(nameof(Description));
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
		public string? Description => Station?.Description;
		
		public async void OnBikeSelected(Bike bike)
		{
			var bikeVM = App.GetViewModel<BikeViewModel>();
			
			var actions = new (string, ICommand) []
			{
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
	}
}
