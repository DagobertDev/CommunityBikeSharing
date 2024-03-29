﻿using System;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using CommunityBikeSharing.Models;
using Plugin.FirebaseAuth;

namespace CommunityBikeSharing.Services
{
	public class FirebaseAuthService : IAuthService
	{
		private readonly IAuth _auth = CrossFirebaseAuth.Current.Instance;
		private IObservable<User?>? _user;

		public async Task<User?> Register(string email, string password)
		{
			if (string.IsNullOrEmpty(email))
			{
				throw new AuthError(AuthError.AuthErrorReason.MissingEmail);
			}

			if (string.IsNullOrEmpty(password))
			{
				throw new AuthError(AuthError.AuthErrorReason.MissingPassword);
			}

			IUser? user;

			try
			{
				var result = await _auth.CreateUserWithEmailAndPasswordAsync(email, password);
				user = result.User;
			}
			catch (FirebaseAuthException e)
			{
				throw e.ErrorType switch
				{
					ErrorType.WeakPassword => new AuthError(AuthError.AuthErrorReason.WeakPassword, e),
					ErrorType.UserCollision => new AuthError(AuthError.AuthErrorReason.EmailAlreadyUsed, e),
					_ => new AuthError(AuthError.AuthErrorReason.Undefined, e)
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
			return GetCurrentUser();
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
				throw e.ErrorType switch
				{
					ErrorType.InvalidUser => new AuthError(AuthError.AuthErrorReason.UnknownEmailAddress, e),
					ErrorType.InvalidCredentials => new AuthError(AuthError.AuthErrorReason.WrongPassword, e),
					_ => new AuthError(AuthError.AuthErrorReason.Undefined, e)
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
				throw e.ErrorType switch
				{
					ErrorType.InvalidUser => new AuthError(AuthError.AuthErrorReason.UnknownEmailAddress, e),
					_ => new AuthError(AuthError.AuthErrorReason.Undefined, e)
				};
			}
		}

		public async Task ChangePassword(string newPassword)
		{
			var user = _auth.CurrentUser;

			if (user == null)
			{
				throw new NullReferenceException(nameof(IAuth.CurrentUser));
			}

			try
			{
				await user.UpdatePasswordAsync(newPassword);
			}
			catch (FirebaseAuthException e)
			{
				throw e.ErrorType switch
				{
					ErrorType.WeakPassword => new AuthError(AuthError.AuthErrorReason.WeakPassword, e),
					_ => new AuthError(AuthError.AuthErrorReason.Undefined, e)
				};
			}
		}

		public async Task UpdateUsername(string name)
		{
			var user = _auth.CurrentUser;

			if (user == null)
			{
				throw new NullReferenceException(nameof(IAuth.CurrentUser));
			}

			try
			{
				await user.UpdateProfileAsync(new UserProfileChangeRequest
				{
					DisplayName = name
				});
				await user.GetIdTokenAsync(true);
			}
			catch (FirebaseAuthException e)
			{
				throw e.ErrorType switch
				{
					ErrorType.InvalidCredentials => new AuthError(AuthError.AuthErrorReason.UnknownEmailAddress, e),
					_ => new AuthError(AuthError.AuthErrorReason.Undefined, e)
				};
			}
		}

		public async Task UpdateEmail(string email)
		{
			var user = _auth.CurrentUser;

			if (user == null)
			{
				throw new NullReferenceException(nameof(IAuth.CurrentUser));
			}

			try
			{
				await user.UpdateEmailAsync(email); 
			}
			catch (FirebaseAuthException e)
			{
				throw e.ErrorType switch
				{
					ErrorType.InvalidCredentials => new AuthError(AuthError.AuthErrorReason.UnknownEmailAddress, e),
					ErrorType.UserCollision => new AuthError(AuthError.AuthErrorReason.EmailAlreadyUsed, e),
					_ => new AuthError(AuthError.AuthErrorReason.Undefined, e)
				};
			}
		}

		public string GetCurrentUserId() => _auth.CurrentUser?.Uid ?? string.Empty;

		public IObservable<User?> ObserveCurrentUser()
		{
			if (_user == null)
			{
				var user = new Subject<User?>();

				_auth.IdToken += (sender, args) =>
				{
					user.OnNext(SafeGetCurrentUser());
				};

				_auth.AuthState += (sender, args) =>
				{
					user.OnNext(SafeGetCurrentUser());
				};

				_user = user.StartWith(SafeGetCurrentUser()).Replay(1).RefCount();
			}

			return _user;
		}

		private User? SafeGetCurrentUser() => SignedIn ? GetCurrentUser() : null;
		public User GetCurrentUser()
		{
			var user = _auth.CurrentUser;
			
			if (user == null)
			{
				throw new NullReferenceException(nameof(IAuth.CurrentUser));
			}

			return new User
			{
				Id = user.Uid,
				Username = user.DisplayName ?? user.Email ?? throw new NullReferenceException(nameof(IUser.DisplayName))
			};
		}

		public UserData GetCurrentUserData()
		{
			var user = _auth.CurrentUser;
			
			if (user == null)
			{
				throw new NullReferenceException(nameof(IAuth.CurrentUser));
			}

			return new UserData
			{
				Email = user.Email ?? throw new NullReferenceException(nameof(IUser.Email))
			};
		}

		public async Task<bool> Reauthenticate(string email, string password)
		{
			var user = _auth.CurrentUser;
			
			if (user == null)
			{
				throw new NullReferenceException(nameof(IAuth.CurrentUser));
			}
			
			var credential = CrossFirebaseAuth.Current.EmailAuthProvider.GetCredential(email, password);
			
			try
			{
				await user.ReauthenticateAsync(credential);
				return true;
			}
			catch (Exception)
			{
				return false;
			}
		}

		public Task DeleteCurrentUser()
		{
			var user = _auth.CurrentUser;
			
			if (user == null)
			{
				throw new NullReferenceException(nameof(IAuth.CurrentUser));
			}
			
			return user.DeleteAsync();
		}

		public bool SignedIn => _auth.CurrentUser != null;
	}
}
