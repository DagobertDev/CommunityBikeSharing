#nullable enable
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CommunityBikeSharing.Models;

namespace CommunityBikeSharing.Services.Data.Memberships
{
	public interface IMembershipService
	{
		Task<CommunityMembership> Get(string community, string user);
		IObservable<CommunityMembership> Observe(string community);
		Task<ICollection<CommunityMembership>> GetMembershipsFromCommunity(string community);
		ObservableCollection<CommunityMembership> ObserveMembershipsFromCommunity(string communityId);
		ObservableCollection<CommunityMembership> ObserveMemberships();
		Task<CommunityMembership> Add(User user, string community);
		Task PromoteToCommunityAdmin(CommunityMembership membership);
		Task Delete(CommunityMembership membership);
	}
}
