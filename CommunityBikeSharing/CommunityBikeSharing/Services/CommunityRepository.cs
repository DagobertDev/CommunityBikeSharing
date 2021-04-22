using System.Collections.ObjectModel;
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

		public CommunityRepository()
		{
			_authService = DependencyService.Get<IAuthService>();
		}

		public ObservableCollection<Community> ObserveCommunities()
		{
			var user = _authService.User;

			var result = new ObservableCollection<Community>();

			CrossCloudFirestore.Current.Instance.Collection("communities")
				.WhereArrayContains(nameof(Community.UserIds), user.Id)

				.AddSnapshotListener((snapshot, error) =>
                   {
	                   result.Clear();

	                   if (snapshot == null)
	                   {
		                   return;
	                   }

	                   foreach (var data in snapshot.ToObjects<Community>())
                       {
	                       result.Add(data);
                       }
                   });

			return result;
		}

		public async void AddCommunity(Community community)
		{
			var user = _authService.User;

			community.UserIds = new[] {user.Id};
			await CrossCloudFirestore.Current.Instance.Collection("communities").AddAsync(community);
		}
	}
}
