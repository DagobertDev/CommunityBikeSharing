using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CommunityBikeSharing.Models;

namespace CommunityBikeSharing.Services
{
	public interface IBikeRepository
	{
		public Task<Bike> Add(Bike bike);
		public Task<Bike> Add(string name, string communityId)
			=> Add(new Bike {Name = name, CommunityId = communityId});
		public Task Update(Bike bike);
		public Task Delete(Bike bike);
		public ObservableCollection<Bike> ObserveBikesFromCommunity(string communityId);
	}
}
