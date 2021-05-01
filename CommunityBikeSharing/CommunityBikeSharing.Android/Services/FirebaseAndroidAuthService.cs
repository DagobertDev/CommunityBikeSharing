using System.Threading.Tasks;
using CommunityBikeSharing.Models;
using CommunityBikeSharing.Services;
using Firebase;
using Firebase.Auth;

namespace CommunityBikeSharing.Droid.Services
{
	public class FirebaseAndroidAuthService : IAuthService
	{
		private readonly FirebaseAuth _firebaseAuth;

		public FirebaseAndroidAuthService()
		{
			_firebaseAuth = FirebaseAuth.Instance;
		}

		public async Task<User> Register(string email, string password)
		{
			if (string.IsNullOrEmpty(email))
			{
				throw new AuthError(AuthError.AuthErrorReason.MissingEmail);
			}

			if (string.IsNullOrEmpty(password))
			{
				throw new AuthError(AuthError.AuthErrorReason.MissingPassword);
			}

			try
			{
				var result = await _firebaseAuth.CreateUserWithEmailAndPasswordAsync(email, password);
				return new User {Email = result.User.Email, Id = result.User.Uid, Username = result.User.DisplayName};
			}
			catch (FirebaseAuthWeakPasswordException)
			{
				throw new AuthError(AuthError.AuthErrorReason.WeakPassword);
			}
			catch (FirebaseAuthUserCollisionException)
			{
				throw new AuthError(AuthError.AuthErrorReason.EmailAlreadyUsed);
			}
			catch (FirebaseException)
			{
				throw new AuthError(AuthError.AuthErrorReason.Undefined);
			}
		}

		public async Task SignIn(string email, string password)
		{
			if (string.IsNullOrEmpty(email))
			{
				throw new AuthError(AuthError.AuthErrorReason.MissingEmail);
			}

			if (string.IsNullOrEmpty(password))
			{
				throw new AuthError(AuthError.AuthErrorReason.MissingPassword);
			}

			try
			{
				await _firebaseAuth.SignInWithEmailAndPasswordAsync(email, password);
			}
			catch (FirebaseAuthInvalidUserException)
			{
				throw new AuthError(AuthError.AuthErrorReason.UnknownEmailAddress);
			}
			catch (FirebaseAuthInvalidCredentialsException)
			{
				throw new AuthError(AuthError.AuthErrorReason.WrongPassword);
			}
			catch (FirebaseAuthException)
			{
				throw new AuthError(AuthError.AuthErrorReason.Undefined);
			}
		}

		public Task SignOut()
		{
			_firebaseAuth.SignOut();
			return Task.CompletedTask;
		}


		public async Task ResetPassword(string email)
		{
			if (string.IsNullOrEmpty(email))
			{
				throw new AuthError(AuthError.AuthErrorReason.MissingEmail);
			}

			try
			{
				await _firebaseAuth.SendPasswordResetEmailAsync(email);
			}
			catch (FirebaseAuthInvalidCredentialsException)
			{
				throw new AuthError(AuthError.AuthErrorReason.UnknownEmailAddress);
			}
			catch (FirebaseAuthException)
			{
				throw new AuthError(AuthError.AuthErrorReason.Undefined);
			}
		}

		public string GetCurrentUserId() => _firebaseAuth.CurrentUser?.Uid ?? string.Empty;

		public bool SignedIn => _firebaseAuth.CurrentUser != null;
	}
}
