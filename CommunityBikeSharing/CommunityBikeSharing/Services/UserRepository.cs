using System.Linq;
using System.Threading.Tasks;
using CommunityBikeSharing.Models;
using CommunityBikeSharing.Services;
using Plugin.CloudFirestore;
using Xamarin.Forms;

[assembly: Dependency(typeof(UserRepository))]

namespace CommunityBikeSharing.Services
{
	public class UserRepository : IUserRepository
	{
		private readonly IFirestore _firestore;

		private ICollectionReference Users => _firestore.Collection("users");

		public UserRepository()
		{
			_firestore = DependencyService.Get<IFirestore>();
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

		public async Task<User> CreateUser(User user)
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
	}
}
