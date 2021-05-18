#nullable enable
using System.Threading.Tasks;
using CommunityBikeSharing.Models;
using Plugin.CloudFirestore;

namespace CommunityBikeSharing.Services.Data.Users
{
	public class UserRepository : FirestoreRepository<User>, IUserRepository
	{
		private readonly IFirestoreContext _firestore;
		private ICollectionReference Users => _firestore.Users;

		public UserRepository(IFirestoreContext firestoreContext)
		{
			_firestore = firestoreContext;
		}

		public async Task<User> Get(string id)
		{
			var result = await Users.Document(id).GetAsync();
			return result.ToObject<User>()!;
		}

		protected override IDocumentReference GetDocument(User user) => Users.Document(user.Id);
		protected override IDocumentReference GetNewDocument(User user) => Users.Document(user.Id);
		protected override ICollectionReference GetCollection(User user) => Users;
	}
}
