#nullable enable
using System.Collections.ObjectModel;
using CommunityBikeSharing.Models;

namespace CommunityBikeSharing.Services
{
	public interface IStationService
	{
		public ObservableCollection<Station> GetAvailableStations();
	}
}
