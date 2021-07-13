using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Threading.Tasks;
using CommunityBikeSharing.Models;

namespace CommunityBikeSharing.Services.Data.Memberships
{
	public class MembershipService : IMembershipService
	{
		private readonly IMembershipRepository _membershipRepository;
		private readonly IAuthService _authService;

		public MembershipService(
			IMembershipRepository membershipRepository,
			IAuthService authService)
		{
			_membershipRepository = membershipRepository;
			_authService = authService;
		}

		public Task<CommunityMembership> Get(string community, string user) => _membershipRepository.Get(community, user);

		public IObservable<CommunityMembership> Observe(string community)
		{
			var user = _authService.GetCurrentUser();
			return _membershipRepository.Observe(community, user);
		}

		public Task<ICollection<CommunityMembership>> GetMembershipsFromCommunity(string community) =>
			_membershipRepository.GetMembershipsFromCommunity(community);

		public IObservable<ICollection<CommunityMembership>> ObserveMembershipsFromCommunity(string communityId) =>
			_membershipRepository.ObserveMembershipsFromCommunity(communityId);

		public ObservableCollection<CommunityMembership> ObserveMemberships()
		{
			var result = new ObservableCollection<CommunityMembership>();
			ObservableCollection<CommunityMembership>? currentMemberships = null;

			_authService.ObserveCurrentUser().Subscribe(user =>
				{
					result.Clear();

					if (currentMemberships != null)
					{
						currentMemberships.CollectionChanged -= CurrentMembershipsOnCollectionChanged;
					}

					if (user?.Id == null)
					{
						return;
					}

					currentMemberships = _membershipRepository.ObserveMembershipsFromUser(user.Id);

					foreach (var membership in currentMemberships)
					{
						result.Add(membership);
					}

					currentMemberships.CollectionChanged += CurrentMembershipsOnCollectionChanged;
				},
				onError => result.Clear());

			return result;

			void CurrentMembershipsOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
			{
				result.Clear();

				foreach (var membership in currentMemberships)
				{
					result.Add(membership);
				}
			}
		}


		public Task<CommunityMembership> Add(User user, string community)
		{
			var membership = new CommunityMembership
			{
				Name = user.Username,
				UserId = user.Id,
				CommunityId = community,
				Role = CommunityRole.User
			};

			return _membershipRepository.Add(membership);
		}

		public Task PromoteToCommunityAdmin(CommunityMembership membership)
		{
			membership.Role = CommunityRole.CommunityAdmin;
			return _membershipRepository.Update(membership);
		}

		public Task Delete(CommunityMembership membership) => _membershipRepository.Delete(membership);
	}
}
