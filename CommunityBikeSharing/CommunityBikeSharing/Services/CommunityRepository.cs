using System.Collections.ObjectModel;
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
		private readonly IFirestore _firestore;
		private ObservableCollection<Community> _observedCommunities;

		private ICollectionReference CommunityUsers => _firestore.Collection("communityUsers");
		private ICollectionReference Communities => _firestore.Collection("communities");

		public CommunityRepository()
		{
			_userService = DependencyService.Get<IUserService>();
			_firestore = DependencyService.Get<IFirestore>();
		}

		public async Task<ObservableCollection<Community>> GetCommunities()
		{
			if (_observedCommunities == null)
			{
				_observedCommunities = new ObservableCollection<Community>();

				var user = await _userService.GetCurrentUser();

				var communityUsers = (await CommunityUsers
					.WhereEqualsTo(nameof(CommunityMember.UserId), user.Id).GetAsync()).ToObjects<CommunityMember>();

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

			_observedCommunities.Add(community);

			await AddUserToCommunity(await _userService.GetCurrentUser(), community.Id, CommunityRole.CommunityAdmin);
		}

		public async Task AddUserToCommunity(User user, string communityId, CommunityRole role = CommunityRole.User)
		{
			var communityUser = new CommunityMember
			{
				UserId = user.Id,
				CommunityId = communityId,
				Name = user.Username,
				Role = role
			};

			await CommunityUsers.AddAsync(communityUser);
		}

		public async Task<ObservableCollection<CommunityMember>> GetCommunityMembers(string communityId)
		{
			var result = new ObservableCollection<CommunityMember>();

			var querySnapshot = await CommunityUsers.WhereEqualsTo(nameof(CommunityMember.CommunityId), communityId).GetAsync();

			foreach (var communityUser in querySnapshot.ToObjects<CommunityMember>())
			{
				result.Add(communityUser);
			}

			return result;
		}
	}
}
