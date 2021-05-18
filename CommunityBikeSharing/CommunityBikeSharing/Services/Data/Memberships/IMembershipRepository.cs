#nullable enable
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CommunityBikeSharing.Models;

namespace CommunityBikeSharing.Services.Data.Memberships
{
	public interface IMembershipRepository : IRepository<CommunityMembership>
	{
		IObservable<CommunityMembership> Observe(string community, User user);
		Task<ICollection<CommunityMembership>> GetMembershipsFromCommunity(string community);
		ObservableCollection<CommunityMembership> ObserveMembershipsFromCommunity(string communityId);
		ObservableCollection<CommunityMembership> ObserveMembershipsFromUser(string userId);
	}
}
