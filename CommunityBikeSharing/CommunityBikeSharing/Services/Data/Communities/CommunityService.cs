using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using CommunityBikeSharing.Models;
using CommunityBikeSharing.Services.Data.Memberships;

namespace CommunityBikeSharing.Services.Data.Communities
{
	public class CommunityService : ICommunityService
	{
		private readonly IFirestoreContext _context;
		private readonly ICommunityRepository _communityRepository;
		private readonly IMembershipRepository _membershipRepository;
		private readonly IMembershipService _membershipService;
		private readonly IAuthService _authService;

		public CommunityService(IFirestoreContext context,
			ICommunityRepository communityRepository,
			IMembershipRepository membershipRepository,
			IMembershipService membershipService,
			IAuthService authService)
		{
			_context = context;
			_communityRepository = communityRepository;
			_membershipRepository = membershipRepository;
			_membershipService = membershipService;
			_authService = authService;
		}

		public async Task<Community> Create(string name, bool showCurrentUser)
		{
			var creator = _authService.GetCurrentUser();

			var communityId = _context.Communities.Document().Id;

			var community = new Community
			{
				Id = communityId,
				Name = name,
				ShowCurrentUser = showCurrentUser
			};

			var membership = new CommunityMembership
			{
				Name = creator.Username,
				Role = CommunityRole.CommunityAdmin,
				CommunityId = communityId,
				UserId = creator.Id
			};

			await _context.RunTransactionAsync(transaction =>
			{
				_communityRepository.Add(community, transaction);
				_membershipRepository.Add(membership, transaction);
			});

			return community;
		}

		public Task Rename(Community community, string name)
			=> _communityRepository.Update(community, nameof(Community.Name), name);

		public Task UpdateReservationDuration(Community community, TimeSpan reservationTime)
		{
			// We can't just update the single field because TimeSpan needs to be converted.
			community.ReserveTime = reservationTime;
			return _communityRepository.Update(community);
		}

		public async Task Delete(Community community)
		{
			var memberships = await _context.CommunityUsers
				.WhereEqualsTo(nameof(CommunityMembership.CommunityId), community.Id)
				.GetAsync();

			var bikes = await _context.Bikes(community.Id).GetAsync();

			await _context.RunTransactionAsync(transaction =>
			{
				transaction.Delete(_context.Communities.Document(community.Id));

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

		public IObservable<Community> Observe(string id) => _communityRepository.Observe(id);

		private ObservableCollection<Community> _userCommunities;
		public ObservableCollection<Community> GetCommunities()
		{
			var memberships = _membershipService.ObserveMemberships();

			if (_userCommunities == null)
			{
				_userCommunities = new ObservableCollection<Community>();

				memberships.CollectionChanged += (sender, args) =>
				{
					_userCommunities.Clear();

					foreach (var membership in memberships)
					{
						var observableCommunity = Observe(membership.CommunityId);

						observableCommunity.Subscribe(
							community => OnCommunityChanged(community, membership.CommunityId),
							exception => {OnCommunityChanged(null, membership.CommunityId);});
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
	}
}
