using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CommunityBikeSharing.Models;

namespace CommunityBikeSharing.Services
{
	public interface IMembershipRepository
	{
		Task<CommunityMembership> Get(Community community, User user);
		Task<ObservableCollection<CommunityMembership>> GetMembershipsByCommunity(string communityId);
		Task Add(CommunityMembership membership);
		Task Update(CommunityMembership membership);
	}
}
