using Plugin.CloudFirestore;

namespace CommunityBikeSharing.Services
{
	public interface IFirestoreContext
	{
		IFirestore Firestore { get; }
		ICollectionReference Users { get; }
		ICollectionReference CommunityUsers { get; }
		ICollectionReference Communities { get; }
	}
}
