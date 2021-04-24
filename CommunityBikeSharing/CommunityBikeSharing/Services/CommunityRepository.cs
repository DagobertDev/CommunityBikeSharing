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
		private readonly IAuthService _authService;
		private ObservableCollection<Community> _observedCommunities;

		public CommunityRepository()
		{
			_authService = DependencyService.Get<IAuthService>();
		}

		public ObservableCollection<Community> ObserveCommunities()
		{
			if (_observedCommunities == null)
			{
				_observedCommunities = new ObservableCollection<Community>();

				var user = _authService.User;

				CrossCloudFirestore.Current.Instance.Collection("communities")
					.WhereArrayContains(nameof(Community.UserIds), user.Id)

					.AddSnapshotListener((snapshot, error) =>
					{
						_observedCommunities.Clear();

						if (snapshot == null)
						{
							return;
						}

						foreach (var data in snapshot.ToObjects<Community>())
						{
							_observedCommunities.Add(data);
						}
					});
			}

			return _observedCommunities;
		}

		public async Task<Community> GetCommunity(string id)
		{
			var result = await CrossCloudFirestore.Current.Instance.Collection("communities").Document(id).GetAsync();
			return result.ToObject<Community>();
		}

		public Task AddCommunity(Community community)
		{
			var user = _authService.User;

			community.UserIds = new[] {user.Id};
			return CrossCloudFirestore.Current.Instance.Collection("communities").AddAsync(community);
		}
	}
}
