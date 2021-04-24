using System;
using System.Threading.Tasks;
using CommunityBikeSharing.iOS.Services;
using CommunityBikeSharing.Models;
using CommunityBikeSharing.Services;
using Firebase.Auth;
using Xamarin.Forms;
using User = CommunityBikeSharing.Models.User;

[assembly: Dependency(typeof(FirebaseIOSAuthService))]

namespace CommunityBikeSharing.iOS.Services
{
	public class FirebaseIOSAuthService : IAuthService
	{
		private readonly Auth _firebaseAuth;

		public FirebaseIOSAuthService()
		{
			_firebaseAuth = Auth.DefaultInstance;
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
				await _firebaseAuth.CreateUserAsync(email, password);
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

		public User User =>
			_firebaseAuth.CurrentUser != null
				? new User {Email = _firebaseAuth.CurrentUser.Email, Id = _firebaseAuth.CurrentUser.Uid}
				: new User();

		public bool SignedIn => _firebaseAuth.CurrentUser != null;
	}
}
