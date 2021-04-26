using CommunityBikeSharing.Services;
using Plugin.CloudFirestore;
using Xamarin.Forms;

[assembly: Dependency(typeof(FirestoreContext))]

namespace CommunityBikeSharing.Services
{
	public class FirestoreContext : IFirestoreContext
	{
		private static readonly IFirestore Firestore = CrossCloudFirestore.Current.Instance;

		public ICollectionReference Users { get; } = Firestore.Collection("users");
		public ICollectionReference CommunityUsers { get; } = Firestore.Collection("communityUsers");
		public ICollectionReference Communities { get; } = Firestore.Collection("communities");
	}
}
