#nullable enable
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CommunityBikeSharing.Models;
using CommunityBikeSharing.Services.Data;

namespace CommunityBikeSharing.ViewModels
{
	public class StationDetailViewModel : BaseViewModel
	{
		public static string[] NavigationParameters(string communityId, string stationId) =>
			new [] {communityId, stationId};

		private readonly IStationRepository _stationRepository;
		private readonly IBikeRepository _bikeRepository;

		private readonly string _communityId;
		private readonly string _stationId;

		public StationDetailViewModel(
			IStationRepository stationRepository,
			IBikeRepository bikeRepository,
			string communityId,
			string stationId)
		{
			_stationRepository = stationRepository;
			_bikeRepository = bikeRepository;
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
			Station = await _stationRepository.Get(_communityId, _stationId);
			Bikes = _bikeRepository.ObserveBikesFromStation(Station);
		}

		public string Name => Station?.Name ?? string.Empty;
	}
}
