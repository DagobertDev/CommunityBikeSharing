using Plugin.CloudFirestore;

namespace CommunityBikeSharing.Services
{
	public class FirestoreContext : IFirestoreContext
	{
		private static readonly IFirestore Instance = CrossCloudFirestore.Current.Instance;

		public IFirestore Firestore => Instance;
		public ICollectionReference Bikes(string communityId) => Communities.Document(communityId).Collection("bikes");
		public ICollectionReference Users { get; } = Instance.Collection("users");
		public ICollectionReference CommunityUsers { get; } = Instance.Collection("communities_users");
		public ICollectionReference Communities { get; } = Instance.Collection("communities");
	}
}
