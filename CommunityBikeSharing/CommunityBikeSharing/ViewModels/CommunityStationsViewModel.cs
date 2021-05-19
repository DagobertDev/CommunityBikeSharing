#nullable enable
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using CommunityBikeSharing.Models;
using CommunityBikeSharing.Services;
using CommunityBikeSharing.Services.Data.Memberships;
using CommunityBikeSharing.Services.Data.Stations;
using Xamarin.Forms;

namespace CommunityBikeSharing.ViewModels
{
	public class CommunityStationsViewModel : BaseViewModel
	{
		private readonly IMembershipService _membershipService;
		private readonly IStationService _stationService;
		private readonly INavigationService _navigationService;
		private string CommunityId { get; }

		public CommunityStationsViewModel(
			IMembershipService membershipService,
			IStationService stationService,
			INavigationService navigationService,
			string communityId)
		{
			_membershipService = membershipService;
			_stationService = stationService;
			_navigationService = navigationService;
			CommunityId = communityId;
		}

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

		private CommunityMembership? _currentUserMembership;

		private CommunityMembership? CurrentUserMembership
		{
			get => _currentUserMembership;
			set
			{
				_currentUserMembership = value;
				OnPropertyChanged();
				OnPropertyChanged(nameof(AddStationVisible));
			}
		}

		public ICommand AddStationCommand => new Command(AddStation);
		private async void AddStation()
		{
			await _navigationService.NavigateTo<EditStationViewModel>(CommunityId);
		}
		public bool AddStationVisible => CurrentUserMembership is {Role: CommunityRole.CommunityAdmin};

		public ICommand EditStationCommand => new Command<Station>(EditStation, CanEditStation);

		private async void EditStation(Station station)
		{
			await _navigationService.NavigateTo<EditStationViewModel>(CommunityId, station.Id);
		}

		private bool CanEditStation(Station station) => CurrentUserMembership is {Role: CommunityRole.CommunityAdmin};

		public override Task InitializeAsync()
		{
			_stationService.ObserveStationsFromCommunity(CommunityId)
				.Subscribe(
					stations => Stations = new ObservableCollection<Station>(stations),
					exception => Stations = null);

			_membershipService.Observe(CommunityId).Subscribe(
				membership => CurrentUserMembership = membership,
				exception => CurrentUserMembership = null);

			return Task.CompletedTask;
		}
	}
}
