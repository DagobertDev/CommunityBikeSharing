using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CommunityBikeSharing.Models;
using CommunityBikeSharing.Services;
using Plugin.CloudFirestore;
using Xamarin.Forms;

[assembly: Dependency(typeof(MembershipRepository))]

namespace CommunityBikeSharing.Services
{
	public class MembershipRepository : IMembershipRepository
	{
		private readonly IFirestoreContext _firestore;
		private ICollectionReference Memberships => _firestore.CommunityUsers;

		public MembershipRepository()
		{
			_firestore = DependencyService.Get<IFirestoreContext>();
		}

		public async Task<CommunityMembership> Get(Community community, User user)
		{
			var snapshot = await Memberships.Document(CommunityMembership.GetId(community, user)).GetAsync();
			return snapshot.ToObject<CommunityMembership>();
		}

		public async Task<ObservableCollection<CommunityMembership>> GetMembershipsByCommunity(string communityId)
		{
			var snapshot = await Memberships.WhereEqualsTo(nameof(CommunityMembership.CommunityId), communityId).GetAsync();

			var result = new ObservableCollection<CommunityMembership>();

			foreach (var membership in snapshot.ToObjects<CommunityMembership>())
			{
				result.Add(membership);
			}

			return result;
		}

		public Task Add(CommunityMembership membership)
			=> Memberships.Document(membership.Id).SetAsync(membership);

		public Task Update(CommunityMembership membership)
			=> Memberships.Document(membership.Id).UpdateAsync(membership);
	}
}
