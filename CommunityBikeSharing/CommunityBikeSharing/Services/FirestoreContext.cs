using CommunityBikeSharing.Services;
using Plugin.CloudFirestore;
using Xamarin.Forms;

[assembly: Dependency(typeof(FirestoreContext))]

namespace CommunityBikeSharing.Services
{
	public class FirestoreContext : IFirestoreContext
	{
		private static readonly IFirestore Instance = CrossCloudFirestore.Current.Instance;

		public IFirestore Firestore { get; } = Instance;
		public ICollectionReference Users { get; } = Instance.Collection("users");
		public ICollectionReference CommunityUsers { get; } = Instance.Collection("communityUsers");
		public ICollectionReference Communities { get; } = Instance.Collection("communities");
	}
}
