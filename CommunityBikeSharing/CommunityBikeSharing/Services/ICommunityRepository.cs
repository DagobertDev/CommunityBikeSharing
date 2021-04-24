using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CommunityBikeSharing.Models;

namespace CommunityBikeSharing.Services
{
	public interface ICommunityRepository
	{
		Task CreateCommunity(string name);
		Task<Community> GetCommunity(string id);
		Task<ObservableCollection<Community>> GetCommunities();
		Task<ObservableCollection<CommunityMember>> GetCommunityMembers(string communityId);
		Task AddUserToCommunity(User user, string communityId, CommunityRole role = CommunityRole.User);
	}
}
