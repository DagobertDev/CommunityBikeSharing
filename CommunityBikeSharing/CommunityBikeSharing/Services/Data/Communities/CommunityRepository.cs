﻿using System;
using System.Reactive.Linq;
using System.Threading.Tasks;
using CommunityBikeSharing.Models;
using Plugin.CloudFirestore;
using Plugin.CloudFirestore.Reactive;

namespace CommunityBikeSharing.Services.Data.Communities
{
	public class CommunityRepository : FirestoreRepository<Community>, ICommunityRepository
	{
		private readonly IFirestoreContext _firestore;
		private ICollectionReference Communities => _firestore.Communities;

		public CommunityRepository(
			IFirestoreContext firestoreContext)
		{
			_firestore = firestoreContext;
		}

		public IObservable<Community> Observe(string id)
			=> Communities.Document(id).AsObservable().Select(snap => snap.ToObject<Community>()!);

		public async Task<Community> Get(string id)
		{
			var snapshot = await Communities.Document(id).GetAsync();
			return snapshot.ToObject<Community>() ?? throw new NullReferenceException(nameof(Community));
		}

		public Community Get(string id, ITransaction transaction) =>
			transaction.Get(Communities.Document(id)).ToObject<Community>()!;

		protected override IDocumentReference GetDocument(Community community) => Communities.Document(community.Id);
		protected override IDocumentReference GetNewDocument(Community community) => Communities.Document(community.Id);
		protected override ICollectionReference GetCollection(Community model) => Communities;
	}
}
