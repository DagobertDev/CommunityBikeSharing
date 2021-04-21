using System.Threading.Tasks;
using Android.Gms.Extensions;
using CommunityBikeSharing.Droid.Services;
using CommunityBikeSharing.Models;
using CommunityBikeSharing.Services;
using Firebase;
using Firebase.Auth;
using Xamarin.Forms;

[assembly: Dependency(typeof(FirebaseAndroidAuthService))]

namespace CommunityBikeSharing.Droid.Services
{
	public class FirebaseAndroidAuthService : IAuthService
	{
		private readonly FirebaseAuth _firebaseAuth;

		public FirebaseAndroidAuthService()
		{
			_firebaseAuth = FirebaseAuth.Instance;
		}

		public async Task Register(string email, string password)
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
				await _firebaseAuth.CreateUserWithEmailAndPasswordAsync(email, password);
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

		public User User
		{
			get
			{
				if (SignedIn)
				{
					return new User
					{
						Email = _firebaseAuth.CurrentUser.Email,
						Username = _firebaseAuth.CurrentUser.DisplayName,
						Id = _firebaseAuth.CurrentUser.Uid
					};
				}

				return new User();
			}
		}

		public async Task<string> GetAccessToken()
		{
			if (SignedIn)
			{
				var token = await _firebaseAuth.CurrentUser.GetIdToken(false).AsAsync<GetTokenResult>();
				return token.Token;
			}

			return string.Empty;
		}

		public bool SignedIn => _firebaseAuth.CurrentUser != null;
	}
}
