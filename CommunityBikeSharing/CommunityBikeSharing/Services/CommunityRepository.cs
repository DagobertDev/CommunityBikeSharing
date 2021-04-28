﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using CommunityBikeSharing.Models;
using CommunityBikeSharing.Services;
using Plugin.CloudFirestore;
using Plugin.CloudFirestore.Reactive;
using Xamarin.Forms;

[assembly: Dependency(typeof(CommunityRepository))]

namespace CommunityBikeSharing.Services
{
	public class CommunityRepository : ICommunityRepository
	{
		private readonly IUserService _userService;
		private readonly IFirestoreContext _firestore;
		private ObservableCollection<Community> _userCommunities;

		private readonly IDictionary<string, IObservable<Community>> _observableCommunities =
			new ConcurrentDictionary<string, IObservable<Community>>();
		private ICollectionReference CommunityUsers => _firestore.CommunityUsers;
		private ICollectionReference Communities => _firestore.Communities;

		public CommunityRepository()
		{
			_userService = DependencyService.Get<IUserService>();
			_firestore = DependencyService.Get<IFirestoreContext>();
		}

		public async Task<ObservableCollection<Community>> GetCommunities()
		{
			if (_userCommunities == null)
			{
				_userCommunities = new ObservableCollection<Community>();

				var user = await _userService.GetCurrentUser();
				var memberships = DependencyService.Get<IMembershipRepository>().ObserveMembershipsFromUser(user.Id);

				memberships.CollectionChanged += (sender, args) =>
				{
					_userCommunities.Clear();

					foreach (var membership in memberships)
					{
						var observableCommunity = GetObservableCommunity(membership.CommunityId);

						observableCommunity.Subscribe(
							community => OnCommunityChanged(community, membership.CommunityId));
					}
				};
			}

			return _userCommunities;

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


		public async Task<Community> GetCommunity(string id)
		{
			var result = await Communities.Document(id).GetAsync();
			return result.ToObject<Community>();
		}

		public async Task CreateCommunity(string name)
		{
			var result = await Communities.AddAsync(new Community
			{
				Name = name
			});

			var community = (await result.GetAsync()).ToObject<Community>();

			if (community == null)
			{
				return;
			}

			await AddUserToCommunity(await _userService.GetCurrentUser(), community.Id, CommunityRole.CommunityAdmin);
		}

		public Task UpdateCommunity(Community community) => Communities.Document(community.Id).UpdateAsync(community);

		public async Task DeleteCommunity(string id)
		{
			var memberships = await CommunityUsers.WhereEqualsTo(nameof(CommunityMembership.CommunityId), id)
				.GetAsync();

			await _firestore.Firestore.RunTransactionAsync(transaction =>
			{
				transaction.Delete(Communities.Document(id));

				foreach (var document in memberships.Documents)
				{
					transaction.Delete(document.Reference);
				}
			});
		}

		public Task AddUserToCommunity(User user, string communityId, CommunityRole role = CommunityRole.User)
		{
			var membershipRepo = DependencyService.Get<IMembershipRepository>();

			var membership = new CommunityMembership
			{
				Name = user.Username, Role = role, CommunityId = communityId, UserId = user.Id
			};

			return membershipRepo.Add(membership);
		}
	}
}
