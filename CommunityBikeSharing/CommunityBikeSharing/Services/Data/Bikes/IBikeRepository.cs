#nullable enable
using System.Collections.ObjectModel;
using CommunityBikeSharing.Models;

namespace CommunityBikeSharing.Services.Data.Bikes
{
	public interface IBikeRepository : IRepository<Bike>
	{
		public ObservableCollection<Bike> ObserveBikesFromCommunity(string communityId);
		public ObservableCollection<Bike> ObserveBikesFromStation(Station station);
	}
}
