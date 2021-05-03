using System.Threading.Tasks;
using CommunityBikeSharing.Models;
using Plugin.CloudFirestore;

namespace CommunityBikeSharing.Services.Data
{
	public class UserRepository : IUserRepository
	{
		private readonly IFirestoreContext _firestore;

		private ICollectionReference Users => _firestore.Users;
		private ICollectionReference UserEmails => _firestore.Firestore.Collection("users_email");

		public UserRepository(IFirestoreContext firestoreContext)
		{
			_firestore = firestoreContext;
		}

		public async Task<User> GetUserByEmail(string email)
		{
			var emailSnapshot = await UserEmails.Document(email).GetAsync();

			var userEmail = emailSnapshot.ToObject<UserEmail>();

			if (userEmail == null)
			{
				return null;
			}

			var result = await Users.Document(userEmail.UserId).GetAsync();

			return result.ToObject<User>();
		}

		public async Task<User> GetUserById(string id)
		{
			var result = await Users.Document(id).GetAsync();
			return result.ToObject<User>();
		}

		public async Task<User> Add(User user)
		{
			if (string.IsNullOrEmpty(user.Id))
			{
				return null;
			}

			var userDocument = Users.Document(user.Id);
			await userDocument.SetAsync(user);

			var userEmail = new UserEmail {UserId = user.Id};

			await UserEmails.Document(user.Email).SetAsync(userEmail);

			var userSnapshot = await userDocument.GetAsync();
			return userSnapshot.ToObject<User>();
		}

		public Task Update(User user) => Users.Document(user.Id).UpdateAsync(user);

		public Task Delete(User user) => Users.Document(user.Id).DeleteAsync();

		private class UserEmail
		{
			public string UserId { get; set; }
		}
	}
}
