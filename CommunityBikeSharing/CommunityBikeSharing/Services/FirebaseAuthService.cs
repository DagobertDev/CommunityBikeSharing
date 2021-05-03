using System.Threading.Tasks;
using CommunityBikeSharing.Models;
using Plugin.FirebaseAuth;

namespace CommunityBikeSharing.Services
{
	public class FirebaseAuthService : IAuthService
	{
		private readonly IAuth _auth = CrossFirebaseAuth.Current.Instance;

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

			IUser user;

			try
			{
				var result = await _auth.CreateUserWithEmailAndPasswordAsync(email, password);
				user = result.User;
			}
			catch (FirebaseAuthException e)
			{
				throw e.Reason switch
				{
					"WeakPassword" => new AuthError(AuthError.AuthErrorReason.WeakPassword),
					"UserCollision" => new AuthError(AuthError.AuthErrorReason.EmailAlreadyUsed),
					_ => new AuthError(AuthError.AuthErrorReason.Undefined)
				};
			}

			if (user == null)
			{
				return null;
			}

			await user.UpdateProfileAsync(new UserProfileChangeRequest
			{
				DisplayName = email
			});

			await user.GetIdTokenAsync(true);

			return new User
			{
				Email = user.Email,
				Id = user.Uid,
				Username = user.DisplayName
			};
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
				await _auth.SignInWithEmailAndPasswordAsync(email, password);
			}
			catch (FirebaseAuthException e)
			{
				throw e.Reason switch
				{
					"InvalidUser" => new AuthError(AuthError.AuthErrorReason.UnknownEmailAddress),
					"InvalidCredentials" => new AuthError(AuthError.AuthErrorReason.WrongPassword),
					_ => new AuthError(AuthError.AuthErrorReason.Undefined)
				};
			}
		}


		public Task SignOut()
		{
			_auth.SignOut();
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
				await _auth.SendPasswordResetEmailAsync(email);
			}
			catch (FirebaseAuthException e)
			{
				throw e.Reason switch
				{
					"InvalidCredentials" => new AuthError(AuthError.AuthErrorReason.UnknownEmailAddress),
					_ => new AuthError(AuthError.AuthErrorReason.Undefined)
				};
			}
		}

		public string GetCurrentUserId() => _auth.CurrentUser?.Uid;

		public bool SignedIn => _auth.CurrentUser != null;
	}
}
