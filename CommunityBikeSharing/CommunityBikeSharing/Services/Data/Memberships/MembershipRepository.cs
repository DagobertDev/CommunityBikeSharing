﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using CommunityBikeSharing.Models;
using Plugin.CloudFirestore;
using Plugin.CloudFirestore.Reactive;

namespace CommunityBikeSharing.Services.Data.Memberships
{
	public class MembershipRepository : FirestoreRepository<CommunityMembership>, IMembershipRepository
	{
		private readonly IFirestoreContext _firestore;
		private ICollectionReference Memberships => _firestore.CommunityUsers;

		public MembershipRepository(IFirestoreContext firestoreContext)
		{
			_firestore = firestoreContext;
		}

		public async Task<CommunityMembership> Get(string community, string user)
		{
			var snapshot = await Memberships.Document(CommunityMembership.GetId(community, user)).GetAsync();
			return snapshot.ToObject<CommunityMembership>()!;
		}

		public IObservable<CommunityMembership> Observe(string community, User user)
			=> Memberships.Document(CommunityMembership.GetId(community, user.Id))
				.AsObservable().Select(snapshot => snapshot.ToObject<CommunityMembership>()!);

		public async Task<ICollection<CommunityMembership>> GetMembershipsFromUser(string user)
		{
			var snapshot =
				await Memberships.WhereEqualsTo(nameof(CommunityMembership.UserId), user).GetAsync();

			return snapshot.ToObjects<CommunityMembership>().ToList();
		}

		public async Task<ICollection<CommunityMembership>> GetMembershipsFromCommunity(string community)
		{
			var doc =
				await Memberships.WhereEqualsTo(nameof(CommunityMembership.CommunityId), community).GetAsync();

			return doc.ToObjects<CommunityMembership>().ToList();
		}

		public IObservable<ICollection<CommunityMembership>> ObserveMembershipsFromCommunity(string communityId)
			=> ObservableCommunities(Memberships.WhereEqualsTo(nameof(CommunityMembership.CommunityId), communityId));

		public ObservableCollection<CommunityMembership> ObserveMembershipsFromUser(string userId)
			=> ObserveCommunities(Memberships.WhereEqualsTo(nameof(CommunityMembership.UserId), userId));

		private static ObservableCollection<CommunityMembership> ObserveCommunities(IQuery query)
		{
			var result = new ObservableCollection<CommunityMembership>();

			query.AsObservable().Subscribe(snapshot =>
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
		
		private static IObservable<ICollection<CommunityMembership>> ObservableCommunities(IQuery query) 
			=> query.AsObservable()
				.Select(snapshot => snapshot.ToObjects<CommunityMembership>().ToList())
				.Catch<ICollection<CommunityMembership>>(Observable.Return(Array.Empty<CommunityMembership>()));
		


		protected override IDocumentReference GetDocument(CommunityMembership membership) => Memberships.Document(membership.Id);
		protected override IDocumentReference GetNewDocument(CommunityMembership membership) => Memberships.Document(membership.Id);
		protected override ICollectionReference GetCollection(CommunityMembership model) => Memberships;
	}
}
