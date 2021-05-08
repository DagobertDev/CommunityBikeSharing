using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using CommunityBikeSharing.Models;
using Plugin.CloudFirestore;
using Plugin.CloudFirestore.Reactive;

namespace CommunityBikeSharing.Services.Data
{
	public class MembershipRepository : IMembershipRepository
	{
		private readonly IFirestoreContext _firestore;
		private ICollectionReference Memberships => _firestore.CommunityUsers;
		public MembershipRepository(IFirestoreContext firestoreContext)
		{
			_firestore = firestoreContext;
		}

		public IObservable<CommunityMembership> Observe(string community, User user)
			=> Memberships.Document(CommunityMembership.GetId(community, user.Id))
				.AsObservable().Select(snapshot => snapshot.ToObject<CommunityMembership>());

		public async Task<ICollection<CommunityMembership>> GetMembershipsFromCommunity(string community)
		{
			var doc =
				await Memberships.WhereEqualsTo(nameof(CommunityMembership.CommunityId), community).GetAsync();

			return doc.ToObjects<CommunityMembership>().ToList();
		}

		public ObservableCollection<CommunityMembership> ObserveMembershipsFromCommunity(string communityId)
		{
			var result = new ObservableCollection<CommunityMembership>();

			Memberships.WhereEqualsTo(nameof(CommunityMembership.CommunityId), communityId).AsObservable().Subscribe(
				snapshot =>
				{
					result.Clear();

					foreach (var membership in snapshot.ToObjects<CommunityMembership>())
					{
						result.Add(membership);
					}
				},
				exception =>
				{
					result.Clear();
				});

			return result;
		}

		public ObservableCollection<CommunityMembership> ObserveMembershipsFromUser(string userId)
		{
			var result = new ObservableCollection<CommunityMembership>();

			Memberships.WhereEqualsTo(nameof(CommunityMembership.UserId), userId).AsObservable().Subscribe(snapshot =>
			{
				result.Clear();

				foreach (var membership in snapshot.ToObjects<CommunityMembership>())
				{
					result.Add(membership);
				}
			},
				exception =>
			{
				result.Clear();
			});

			return result;
		}

		public async Task<CommunityMembership> Add(CommunityMembership membership)
		{
			if (membership.Id == null)
			{
				return null;
			}

			var result = await Memberships.Document(membership.Id).GetAsync();

			if (result.Exists)
			{
				return result.ToObject<CommunityMembership>();
			}

			await Memberships.Document(membership.Id).SetAsync(membership);
			return membership;
		}

		public Task Update(CommunityMembership membership)
			=> Memberships.Document(membership.Id).UpdateAsync(membership);

		public Task Delete(CommunityMembership membership)
			=> Memberships.Document(membership.Id).DeleteAsync();
	}
}
