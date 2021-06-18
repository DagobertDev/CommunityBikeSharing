#nullable enable
using System.Threading.Tasks;
using Plugin.CloudFirestore;

namespace CommunityBikeSharing.Services.Data
{
	public interface IFirestoreContext
	{
		IFirestore Firestore { get; }
		ICollectionReference Bikes(string communityId);
		ICollectionReference Stations(string communityId);
		ICollectionReference Locks(string communityId);
		ICollectionReference Users { get; }
		ICollectionReference CommunityUsers { get; }
		ICollectionReference Communities { get; }
		ICollectionReference UserEmails { get; }
		public Task RunTransactionAsync(TransactionHandler handler) => Firestore.RunTransactionAsync(handler);
	}
}
