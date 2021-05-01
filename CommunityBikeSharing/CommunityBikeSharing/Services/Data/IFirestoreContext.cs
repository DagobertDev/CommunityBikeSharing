using Plugin.CloudFirestore;

namespace CommunityBikeSharing.Services.Data
{
	public interface IFirestoreContext
	{
		IFirestore Firestore { get; }
		ICollectionReference Bikes(string communityId);
		ICollectionReference Users { get; }
		ICollectionReference CommunityUsers { get; }
		ICollectionReference Communities { get; }
	}
}
