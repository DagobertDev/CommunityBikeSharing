using System.Linq;
using System.Threading.Tasks;
using CommunityBikeSharing.Models;
using Plugin.CloudFirestore;

namespace CommunityBikeSharing.Services.Data
{
	public class UserRepository : IUserRepository
	{
		private readonly IFirestoreContext _firestore;

		private ICollectionReference Users => _firestore.Users;

		public UserRepository(IFirestoreContext firestoreContext)
		{
			_firestore = firestoreContext;
		}

		public async Task<User> GetUserByEmail(string email)
		{
			var result = await Users.WhereEqualsTo(nameof(User.Email), email).GetAsync();

			var user = result.ToObjects<User>().FirstOrDefault();
			return user;
		}

		public async Task<User> GetUserById(string id)
		{
			var result = await Users.Document(id).GetAsync();
			return result.ToObject<User>();
		}

		public async Task<User> Add(User user)
		{
			IDocumentReference userDocument;

			if (string.IsNullOrEmpty(user.Id))
			{
				userDocument = await Users.AddAsync(user);
			}
			else
			{
				userDocument = Users.Document(user.Id);
				await userDocument.SetAsync(user);
			}

			var userSnapshot = await userDocument.GetAsync();
			return userSnapshot.ToObject<User>();
		}

		public Task Update(User user) => Users.Document(user.Id).UpdateAsync(user);

		public Task Delete(User user) => Users.Document(user.Id).DeleteAsync();
	}
}
