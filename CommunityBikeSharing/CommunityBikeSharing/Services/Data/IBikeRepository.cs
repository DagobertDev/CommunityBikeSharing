using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CommunityBikeSharing.Models;

namespace CommunityBikeSharing.Services.Data
{
	public interface IBikeRepository : IRepository<Bike>
	{
		public Task<Bike> Add(string name, string communityId)
			=> Add(new Bike {Name = name, CommunityId = communityId});
		public ObservableCollection<Bike> ObserveBikesFromCommunity(string communityId);
		public ObservableCollection<Bike> ObserveBikesFromStation(Station station);
	}
}
