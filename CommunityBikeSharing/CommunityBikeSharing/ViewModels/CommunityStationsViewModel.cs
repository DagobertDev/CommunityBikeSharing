using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using CommunityBikeSharing.Models;
using CommunityBikeSharing.Services;
using CommunityBikeSharing.Services.Data.Memberships;
using CommunityBikeSharing.Services.Data.Stations;

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

			AddStationCommand = CreateCommand(AddStation);
			EditStationCommand = CreateCommand<Station>(EditStation, CanEditStation);

			PropertyChanged += (_, args) =>
			{
				switch (args.PropertyName)
				{
					case nameof(CurrentUserMembership):
						OnPropertyChanged(nameof(AddStationVisible));
						break;
				}
			};
		}
		
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
		
		public ICommand AddStationCommand { get; }
		public ICommand EditStationCommand { get; }

		private ObservableCollection<Station>? _stations;
		public ObservableCollection<Station>? Stations
		{
			get => _stations;
			set => SetProperty(ref _stations, value);
		}

		private CommunityMembership? _currentUserMembership;
		private CommunityMembership? CurrentUserMembership
		{
			get => _currentUserMembership;
			set => SetProperty(ref _currentUserMembership, value);
		}

		private Task AddStation() => _navigationService.NavigateTo<EditStationViewModel>(CommunityId);
		public bool AddStationVisible => CurrentUserMembership is {Role: CommunityRole.CommunityAdmin};
		
		private Task EditStation(Station station)
			=> _navigationService.NavigateTo<EditStationViewModel>(CommunityId, station.Id);
		private bool CanEditStation(Station station) => CurrentUserMembership is {Role: CommunityRole.CommunityAdmin};
	}
}
