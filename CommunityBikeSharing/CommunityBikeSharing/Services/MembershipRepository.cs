using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CommunityBikeSharing.Models;
using Plugin.CloudFirestore;
using Plugin.CloudFirestore.Reactive;

namespace CommunityBikeSharing.Services
{
	public class MembershipRepository : IMembershipRepository
	{
		private readonly IFirestoreContext _firestore;
		private ICollectionReference Memberships => _firestore.CommunityUsers;

		private readonly IDictionary<string, ObservableCollection<CommunityMembership>> _cachedMemberships =
			new ConcurrentDictionary<string, ObservableCollection<CommunityMembership>>();

		public MembershipRepository(IFirestoreContext firestoreContext)
		{
			_firestore = firestoreContext;
		}

		public async Task<CommunityMembership> Get(Community community, User user)
		{
			var snapshot = await Memberships.Document(CommunityMembership.GetId(community, user)).GetAsync();
			return snapshot.ToObject<CommunityMembership>();
		}

		public ObservableCollection<CommunityMembership> ObserveMembershipsFromCommunity(string communityId)
		{
			if(_cachedMemberships.TryGetValue(communityId, out var result))
			{
				return result;
			}

			result = new ObservableCollection<CommunityMembership>();
			_cachedMemberships.Add(communityId, result);

			Memberships.WhereEqualsTo(nameof(CommunityMembership.CommunityId), communityId)
				.AsObservable().Subscribe(snapshot =>
				{
					result.Clear();

					foreach (var membership in snapshot.ToObjects<CommunityMembership>())
					{
						result.Add(membership);
					}
				});

			return result;
		}

		public ObservableCollection<CommunityMembership> ObserveMembershipsFromUser(string userId)
		{
			var result = new ObservableCollection<CommunityMembership>();

			Memberships.WhereEqualsTo(nameof(CommunityMembership.UserId), userId)
				.AsObservable().Subscribe(snapshot =>
				{
					result.Clear();

					foreach (var membership in snapshot.ToObjects<CommunityMembership>())
					{
						result.Add(membership);
					}
				});

			return result;
		}

		public Task Add(CommunityMembership membership)
			=> Memberships.Document(membership.Id).SetAsync(membership);

		public Task Update(CommunityMembership membership)
			=> Memberships.Document(membership.Id).UpdateAsync(membership);

		public Task Delete(CommunityMembership membership)
			=> Memberships.Document(membership.Id).DeleteAsync();
	}
}
