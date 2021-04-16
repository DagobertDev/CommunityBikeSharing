using System.Threading.Tasks;
using CommunityBikeSharing.Services;
using Firebase.Auth;
using Newtonsoft.Json;
using Xamarin.Essentials;
using Xamarin.Forms;
using User = CommunityBikeSharing.Models.User;

[assembly: Dependency(typeof(FirebaseAuthService))]

namespace CommunityBikeSharing.Services
{
	public class FirebaseAuthService : IAuthService
	{
		private const string StorageKey = "AuthLink";

		private FirebaseAuthLink _authLink;

		private readonly IFirebaseAuthProvider _authProvider;

		public FirebaseAuthService()
		{
			_authProvider = DependencyService.Get<IFirebaseAuthProvider>();

			var secret = SecureStorage.GetAsync(StorageKey).Result;

			if (secret != null)
			{
				_authLink = JsonConvert.DeserializeObject<FirebaseAuthLink>(secret);
			}
		}

		public Task Register(string email, string password) =>
			_authProvider.CreateUserWithEmailAndPasswordAsync(email, password);

		public async Task SignIn(string email, string password)
		{
			_authLink = await _authProvider.SignInWithEmailAndPasswordAsync(email, password);
			await SecureStorage.SetAsync(StorageKey, JsonConvert.SerializeObject(_authLink));
		}

		public Task SignOut() => Task.FromResult(SecureStorage.Remove(StorageKey));

		public Task ResetPassword(string email) => _authProvider.SendPasswordResetEmailAsync(email);

		private User _user;

		public User User
		{
			get
			{
				if (_user == null && _authLink != null)
				{
					_user = new User {Email = _authLink?.User.Email};
				}

				return _user;
			}
		}

		public bool SignedIn => User != null;
	}
}
