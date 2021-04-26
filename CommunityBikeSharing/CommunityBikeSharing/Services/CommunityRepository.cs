using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using CommunityBikeSharing.Models;
using CommunityBikeSharing.Services;
using Plugin.CloudFirestore;
using Xamarin.Forms;

[assembly: Dependency(typeof(CommunityRepository))]

namespace CommunityBikeSharing.Services
{
	public class CommunityRepository : ICommunityRepository
	{
		private readonly IUserService _userService;
		private readonly IFirestoreContext _firestore;
		private ObservableCollection<Community> _observedCommunities;

		private ICollectionReference CommunityUsers => _firestore.CommunityUsers;
		private ICollectionReference Communities => _firestore.Communities;

		public CommunityRepository()
		{
			_userService = DependencyService.Get<IUserService>();
			_firestore = DependencyService.Get<IFirestoreContext>();
		}

		public async Task<ObservableCollection<Community>> GetCommunities()
		{
			if (_observedCommunities == null)
			{
				_observedCommunities = new ObservableCollection<Community>();

				var user = await _userService.GetCurrentUser();

				var communityUsers = (await CommunityUsers
					.WhereEqualsTo(nameof(CommunityMembership.UserId), user.Id).GetAsync()).ToObjects<CommunityMembership>();

				foreach (var communityUser in communityUsers)
				{
					var community = (await Communities.Document(communityUser.CommunityId).GetAsync()).ToObject<Community>();
					_observedCommunities.Add(community);
				}
			}

			return _observedCommunities;
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

			_observedCommunities?.Add(community);

			await AddUserToCommunity(await _userService.GetCurrentUser(), community.Id, CommunityRole.CommunityAdmin);
		}

		public Task UpdateCommunity(Community community) => Communities.Document(community.Id).UpdateAsync(community);

		public async Task DeleteCommunity(string id)
		{
			await Communities.Document(id).DeleteAsync();

			var community = _observedCommunities?.FirstOrDefault(c => c.Id == id);

			if (community != null)
			{
				_observedCommunities.Remove(community);
			}
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
