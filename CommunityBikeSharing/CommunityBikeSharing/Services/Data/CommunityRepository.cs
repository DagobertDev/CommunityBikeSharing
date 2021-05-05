using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using CommunityBikeSharing.Models;
using Plugin.CloudFirestore;
using Plugin.CloudFirestore.Reactive;

namespace CommunityBikeSharing.Services.Data
{
	public class CommunityRepository : ICommunityRepository
	{
		private readonly IUserService _userService;
		private readonly IFirestoreContext _firestore;

		private ObservableCollection<Community> _userCommunities;

		private readonly IDictionary<string, IObservable<Community>> _observableCommunities =
			new ConcurrentDictionary<string, IObservable<Community>>();

		private ICollectionReference Bikes(string id) => _firestore.Bikes(id);
		private ICollectionReference CommunityUsers => _firestore.CommunityUsers;
		private ICollectionReference Communities => _firestore.Communities;

		public CommunityRepository(
			IUserService userService,
			IFirestoreContext firestoreContext)
		{
			_userService = userService;
			_firestore = firestoreContext;
		}

		public Task<ObservableCollection<Community>> GetCommunities(ObservableCollection<CommunityMembership> memberships)
		{
			if (_userCommunities == null)
			{
				_userCommunities = new ObservableCollection<Community>();

				memberships.CollectionChanged += (sender, args) =>
				{
					_userCommunities.Clear();

					foreach (var membership in memberships)
					{
						var observableCommunity = GetObservableCommunity(membership.CommunityId);

						observableCommunity.Subscribe(
							community => OnCommunityChanged(community, membership.CommunityId),
							exception => {OnCommunityChanged(null, membership.CommunityId);});
					}
				};
			}

			return Task.FromResult(_userCommunities);

			void OnCommunityChanged(Community newCommunity, string id)
			{
				var oldCommunity =
					_userCommunities.SingleOrDefault(c => c.Id == id);

				if (newCommunity == null)
				{
					_userCommunities.Remove(oldCommunity);
					return;
				}

				if (oldCommunity != null)
				{
					_userCommunities[_userCommunities.IndexOf(oldCommunity)] = newCommunity;
				}
				else
				{
					_userCommunities.Add(newCommunity);
				}
			}
		}

		public IObservable<Community> GetObservableCommunity(string id)
		{
			if (_observableCommunities.TryGetValue(id, out var result))
			{
				return result;
			}

			result = Communities.Document(id).AsObservable().Select(s => s.ToObject<Community>());
			_observableCommunities[id] = result;
			return result;
		}


		public IObservable<Community> GetCommunity(string id)
			=> Communities.Document(id).AsObservable().Select(snap => snap.ToObject<Community>());

		public async Task<Community> CreateCommunity(string name)
		{
			var user = await _userService.GetCurrentUser();

			var document = Communities.Document();

			var community = new Community {Name = name};
			var membership = new CommunityMembership
			{
				Name = user.Username,
				Role = CommunityRole.CommunityAdmin,
				CommunityId = document.Id,
				UserId = user.Id
			};

			await _firestore.Firestore.RunTransactionAsync(transaction =>
			{
				transaction.Set(document, community);
				transaction.Set(CommunityUsers.Document(membership.Id), membership);
			});

			var result = await document.GetAsync();
			return result.ToObject<Community>();
		}

		public async Task<Community> Add(Community community)
		{
			var document = await Communities.AddAsync(community);

			var result = await document.GetAsync();

			return result.ToObject<Community>();
		}

		public Task Update(Community community) => Communities.Document(community.Id).UpdateAsync(community);
		public Task Delete(Community community) => Delete(community.Id);

		public async Task Delete(string id)
		{
			var memberships = await CommunityUsers.WhereEqualsTo(nameof(CommunityMembership.CommunityId), id)
				.GetAsync();

			var bikes = await Bikes(id).GetAsync();

			await _firestore.Firestore.RunTransactionAsync(transaction =>
			{
				transaction.Delete(Communities.Document(id));

				foreach (var document in memberships.Documents)
				{
					transaction.Delete(document.Reference);
				}

				foreach (var document in bikes.Documents)
				{
					transaction.Delete(document.Reference);
				}
			});
		}
	}
}
