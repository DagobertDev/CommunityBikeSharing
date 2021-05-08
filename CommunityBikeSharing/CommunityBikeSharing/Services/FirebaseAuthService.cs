using System;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using CommunityBikeSharing.Models;
using CommunityBikeSharing.Services.Data;
using Plugin.FirebaseAuth;

namespace CommunityBikeSharing.Services
{
	public class FirebaseAuthService : IAuthService
	{
		private readonly IAuth _auth = CrossFirebaseAuth.Current.Instance;
		private readonly IUserRepository _userRepository;
		private IObservable<User> _user;

		public FirebaseAuthService(IUserRepository userRepository)
		{
			_userRepository = userRepository;
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

			return await _userRepository.Add(new User
			{
				Id = user.Uid,
				Username = email
			}, email);
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

		public IObservable<User> ObserveCurrentUser()
		{
			if (_user == null)
			{
				var user = new Subject<User>();

				_auth.AuthState += (sender, args) =>
				{
					user.OnNext(GetCurrentUser());
				};

				_user = user;
			}

			return _user.StartWith(GetCurrentUser());
		}

		public User GetCurrentUser()
		{
			if (_auth.CurrentUser == null)
			{
				return null;
			}

			return new User
			{
				Id = _auth.CurrentUser.Uid,
				Username = _auth.CurrentUser.DisplayName
			};
		}

		public bool SignedIn => _auth.CurrentUser != null;
	}
}
