using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using CommunityBikeSharing.Models;
using CommunityBikeSharing.Services;
using CommunityBikeSharing.Services.Data;
using Xamarin.Forms;

namespace CommunityBikeSharing.ViewModels
{
	public class CommunityStationsViewModel : BaseViewModel
	{
		public CommunityStationsViewModel(
			IStationRepository stationRepository,
			IDialogService dialogService,
			string communityId)
		{
			_stationRepository = stationRepository;
			_dialogService = dialogService;
			CommunityId = communityId;
		}

		private readonly IStationRepository _stationRepository;
		private readonly IDialogService _dialogService;
		private string CommunityId { get; }

		private ObservableCollection<Station> _stations;
		public ObservableCollection<Station> Stations
		{
			get => _stations;
			set
			{
				_stations = value;
				OnPropertyChanged();
			}
		}

		public ICommand AddStationCommand => new Command(AddStation);
		private async void AddStation()
		{
			var name = await _dialogService.ShowTextEditor("Name eingeben",
				"Bitte geben Sie einen Namen für die Station ein");

			var station = new Station
			{
				CommunityId = CommunityId,
				Name = name,
			};

			await _stationRepository.Add(station);
		}
		public bool AddStationVisible => true;

		public ICommand EditStationCommand => new Command<Station>(EditStation);

		private async void EditStation(Station station) { }

		public override Task InitializeAsync()
		{
			_stationRepository.ObserveStationsFromCommunity(CommunityId)
				.Subscribe(stations => Stations = new ObservableCollection<Station>(stations));

			return Task.CompletedTask;
		}
	}
}
