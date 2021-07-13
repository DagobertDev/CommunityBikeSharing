using System;
using System.Threading.Tasks;
using CommunityBikeSharing.Models;
using CommunityBikeSharing.Services;
using CommunityBikeSharing.Services.Data.Bikes;
using CommunityBikeSharing.Services.Data.Stations;
using Xamarin.CommunityToolkit.ObjectModel;

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

			PropertyChanged += (_, args) =>
			{
				if (args.PropertyName == nameof(Station))
				{
					OnPropertyChanged(nameof(Name));
					OnPropertyChanged(nameof(Description));
				}
			};
		}
		
		public override async Task InitializeAsync()
		{
			Station = await _stationService.Get(_communityId, _stationId);
			_bikeService.ObserveBikesFromStation(Station).Subscribe(bikes => Bikes.ReplaceRange(bikes));
		}

		private Station? _station;
		private Station? Station
		{
			get => _station;
			set => SetProperty(ref _station, value);
		}

		public ObservableRangeCollection<Bike> Bikes { get; } = new();

		public string Name => Station?.Name ?? string.Empty;
		public string? Description => Station?.Description;
		
		public async void OnBikeSelected(Bike bike)
		{
			var bikeVM = App.GetViewModel<BikeViewModel>();
			
			var actions = new[]
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
