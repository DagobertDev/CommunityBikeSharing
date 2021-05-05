using System;
using System.Collections.ObjectModel;
using CommunityBikeSharing.Models;

namespace CommunityBikeSharing.Services.Data
{
	public interface IMembershipRepository : IRepository<CommunityMembership>
	{
		IObservable<CommunityMembership> Get(Community community, User user);
		ObservableCollection<CommunityMembership> ObserveMembershipsFromCommunity(Community community)
			=> ObserveMembershipsFromCommunity(community.Id);
		ObservableCollection<CommunityMembership> ObserveMembershipsFromCommunity(string communityId);
		ObservableCollection<CommunityMembership> ObserveMembershipsFromUser(string userId);
	}
}
