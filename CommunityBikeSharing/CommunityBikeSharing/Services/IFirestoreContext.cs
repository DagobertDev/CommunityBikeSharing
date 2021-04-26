using Plugin.CloudFirestore;

namespace CommunityBikeSharing.Services
{
	public interface IFirestoreContext
	{
		ICollectionReference Users { get; }
		ICollectionReference CommunityUsers { get; }
		ICollectionReference Communities { get; }
	}
}
