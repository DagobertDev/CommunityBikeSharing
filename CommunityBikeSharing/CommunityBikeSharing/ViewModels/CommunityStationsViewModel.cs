#nullable enable
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using CommunityBikeSharing.Models;
using CommunityBikeSharing.Services;
using CommunityBikeSharing.Services.Data;
using CommunityBikeSharing.Services.Data.Stations;
using Xamarin.Forms;

namespace CommunityBikeSharing.ViewModels
{
	public class CommunityStationsViewModel : BaseViewModel
	{
		public CommunityStationsViewModel(
			IStationService stationService,
			INavigationService navigationService,
			string communityId)
		{
			_stationService = stationService;
			_navigationService = navigationService;
			CommunityId = communityId;
		}

		private readonly IStationService _stationService;
		private readonly INavigationService _navigationService;
		private string CommunityId { get; }

		private ObservableCollection<Station>? _stations;
		public ObservableCollection<Station>? Stations
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
			await _navigationService.NavigateTo<EditStationViewModel>(CommunityId);
		}
		public bool AddStationVisible => true;

		public ICommand EditStationCommand => new Command<Station>(EditStation);

		private async void EditStation(Station station)
		{
			await _navigationService.NavigateTo<EditStationViewModel>(CommunityId, station.Id);
		}

		public override Task InitializeAsync()
		{
			_stationService.ObserveStationsFromCommunity(CommunityId)
				.Subscribe(
					stations => Stations = new ObservableCollection<Station>(stations),
					exception => Stations = null);

			return Task.CompletedTask;
		}
	}
}
