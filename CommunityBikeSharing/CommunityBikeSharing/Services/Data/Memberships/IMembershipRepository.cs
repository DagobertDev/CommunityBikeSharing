﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CommunityBikeSharing.Models;

namespace CommunityBikeSharing.Services.Data.Memberships
{
	public interface IMembershipRepository : IRepository<CommunityMembership>
	{
		Task<CommunityMembership> Get(string community, string user);
		IObservable<CommunityMembership> Observe(string community, User user);
		Task<ICollection<CommunityMembership>> GetMembershipsFromUser(string user);
		Task<ICollection<CommunityMembership>> GetMembershipsFromCommunity(string community);
		IObservable<ICollection<CommunityMembership>> ObserveMembershipsFromCommunity(string communityId);
		ObservableCollection<CommunityMembership> ObserveMembershipsFromUser(string userId);
	}
}
