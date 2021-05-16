#nullable enable
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

		private readonly string _communityId;
		private readonly string _stationId;

		public StationDetailViewModel(
			IStationRepository stationRepository,
			string communityId,
			string stationId)
		{
			_stationRepository = stationRepository;
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

		public override async Task InitializeAsync()
		{
			Station = await _stationRepository.Get(_communityId, _stationId);
		}

		public string Name => Station?.Name ?? string.Empty;
	}
}
