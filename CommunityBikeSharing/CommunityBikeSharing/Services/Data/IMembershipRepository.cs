using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CommunityBikeSharing.Models;

namespace CommunityBikeSharing.Services.Data
{
	public interface IMembershipRepository
	{
		Task<CommunityMembership> Get(Community community, User user);
		ObservableCollection<CommunityMembership> ObserveMembershipsFromCommunity(string communityId);
		ObservableCollection<CommunityMembership> ObserveMembershipsFromUser(string userId);
		Task Add(CommunityMembership membership);
		Task Update(CommunityMembership membership);
		Task Delete(CommunityMembership membership);
	}
}
