using System;
using System.Threading.Tasks;
using CommunityBikeSharing.Models;
using CommunityBikeSharing.Services;
using Firebase.Auth;
using User = CommunityBikeSharing.Models.User;

namespace CommunityBikeSharing.iOS.Services
{
	public class FirebaseIOSAuthService : IAuthService
	{
		private readonly Auth _firebaseAuth;

		public FirebaseIOSAuthService()
		{
			_firebaseAuth = Auth.DefaultInstance;
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
				var result = await _firebaseAuth.CreateUserAsync(email, password);
				return new User {Email = result.User.Email, Id = result.User.Uid, Username = result.User.DisplayName};
			}
			catch (Exception)
			{
				throw new AuthError(AuthError.AuthErrorReason.Undefined);
			}
		}

		public async Task SignIn(string email, string password)
		{
			await _firebaseAuth.SignInWithPasswordAsync(email, password);
		}

		public Task SignOut()
		{
			try
			{
				_ = _firebaseAuth.SignOut(out _);
				return Task.CompletedTask;
			}
			catch (Exception)
			{
				throw new AuthError(AuthError.AuthErrorReason.Undefined);
			}
		}

		public async Task ResetPassword(string email)
		{
			if (string.IsNullOrEmpty(email))
			{
				throw new AuthError(AuthError.AuthErrorReason.MissingEmail);
			}

			try
			{
				await _firebaseAuth.SendPasswordResetAsync(email);
			}
			catch (Exception)
			{
				throw new AuthError(AuthError.AuthErrorReason.Undefined);
			}
		}

		public string GetCurrentUserId() => _firebaseAuth.CurrentUser?.Uid ?? string.Empty;
		public bool SignedIn => _firebaseAuth.CurrentUser != null;
	}
}
