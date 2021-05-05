using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CommunityBikeSharing.Models;

namespace CommunityBikeSharing.Services.Data
{
	public interface IMembershipRepository : IRepository<CommunityMembership>
	{
		IObservable<CommunityMembership> Get(Community community, User user);
		Task<ICollection<CommunityMembership>> GetMembershipsFromCommunity(string community);
		ObservableCollection<CommunityMembership> ObserveMembershipsFromCommunity(Community community)
			=> ObserveMembershipsFromCommunity(community.Id);
		ObservableCollection<CommunityMembership> ObserveMembershipsFromCommunity(string communityId);
		ObservableCollection<CommunityMembership> ObserveMembershipsFromUser(string userId);
	}
}
