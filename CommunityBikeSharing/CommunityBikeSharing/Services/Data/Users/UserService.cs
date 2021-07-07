using System;
using System.Threading.Tasks;
using CommunityBikeSharing.Models;
using CommunityBikeSharing.Services.Data.Memberships;

namespace CommunityBikeSharing.Services.Data.Users
{
	public class UserService : IUserService
	{
		private readonly IFirestoreContext _context;
		private readonly IUserRepository _userRepository;
		private readonly IUserEmailRepository _userEmailRepository;
		private readonly IAuthService _authService;
		private readonly IMembershipRepository _membershipRepository;

		public UserService(IFirestoreContext context,
			IUserRepository userRepository,
			IUserEmailRepository userEmailRepository,
			IMembershipRepository membershipRepository,
			IAuthService authService)
		{
			_context = context;
			_userRepository = userRepository;
			_userEmailRepository = userEmailRepository;
			_membershipRepository = membershipRepository;
			_authService = authService;
		}

		private async Task<User> Add(string authId, string email)
		{
			if (string.IsNullOrEmpty(authId) || string.IsNullOrEmpty(email))
			{
				throw new ArgumentNullException();
			}

			var user = new User {Id = authId, Username = email};
			var userEmail = new UserEmail {Id = email, UserId = authId};

			await _context.RunTransactionAsync(transaction =>
			{
				_userRepository.Add(user, transaction);
				_userEmailRepository.Add(userEmail, transaction);
			});

			return user;
		}

		public async Task<User?> GetUserByEmail(string email)
		{
			var userEmail = await _userEmailRepository.Get(email);

			if (userEmail == null)
			{
				return null;
			}

			return await _userRepository.Get(userEmail.UserId);
		}

		public async Task UpdateEmail(string newMail)
		{
			var user = _authService.GetCurrentUser();
			var userData = _authService.GetCurrentUserData();
			
			await _authService.UpdateEmail(newMail);

			await _context.RunTransactionAsync(transaction =>
			{
				_userEmailRepository.Delete(new UserEmail
				{
					Id = userData.Email,
					UserId = user.Id
				}, transaction);

				_userEmailRepository.Add(new UserEmail
				{
					Id = newMail, 
					UserId = user.Id
				});
			});
		}

		public async Task<User> Register(string email, string password)
		{
			var user = await _authService.Register(email, password);
			return await Add(user.Id, email);
		}

		public async Task DeleteAccount()
		{
			var user = _authService.GetCurrentUser();
			var userData = _authService.GetCurrentUserData();
			
			if (user == null)
			{
				throw new NullReferenceException(nameof(user));
			}
			
			await _context.RunTransactionAsync(transaction =>
			{
				_userRepository.Delete(user, transaction);
				_userEmailRepository.Delete(new UserEmail
				{
					Id = userData.Email,
					UserId = user.Id
				}, transaction);
			});

			await _authService.DeleteCurrentUser();
		}

		public async Task<bool> UpdateUsername(string name)
		{
			await _authService.UpdateUsername(name);

			var user = _authService.GetCurrentUser();

			var memberships = await _membershipRepository.GetMembershipsFromUser(user.Id);

			await _context.RunTransactionAsync(transaction =>
			{
				_userRepository.Update(_authService.GetCurrentUser(), nameof(User.Username), name, transaction);

				foreach (var membership in memberships)
				{
					_membershipRepository.Update(membership, nameof(CommunityMembership.Name), name, transaction);
				}
			});

			return true;
		}
	}
}
