using System;
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

		public Task<User> Add(User user) => throw new NotImplementedException();

		public async Task<User> Add(User user, string email)
		{
			if (string.IsNullOrEmpty(user.Id) || string.IsNullOrEmpty(email))
			{
				return null;
			}

			await _firestore.Firestore.RunTransactionAsync(transaction =>
			{
				transaction.Set(Users.Document(user.Id), user);

				var userEmail = new UserEmail {UserId = user.Id};

				transaction.Set(UserEmails.Document(email), userEmail);
			});

			return user;
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

		public Task Update(User user) => throw new NotImplementedException();

		public Task Delete(User user) => throw new NotImplementedException();

		private class UserEmail
		{
			public string UserId { get; set; }
		}
	}
}
