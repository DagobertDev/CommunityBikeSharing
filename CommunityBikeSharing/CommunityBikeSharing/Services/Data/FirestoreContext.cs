#nullable enable
using Plugin.CloudFirestore;

namespace CommunityBikeSharing.Services.Data
{
	public class FirestoreContext : IFirestoreContext
	{
		private static readonly IFirestore Instance = CrossCloudFirestore.Current.Instance;

		public IFirestore Firestore => Instance;
		public ICollectionReference Bikes(string communityId) => Communities.Document(communityId).Collection("bikes");
		public ICollectionReference Stations(string communityId) => Communities.Document(communityId).Collection("stations");
		public ICollectionReference Users { get; } = Instance.Collection("users");
		public ICollectionReference CommunityUsers { get; } = Instance.Collection("communities_users");
		public ICollectionReference Communities { get; } = Instance.Collection("communities");
		public ICollectionReference UserEmails { get; } = Instance.Collection("users_email");
	}
}
