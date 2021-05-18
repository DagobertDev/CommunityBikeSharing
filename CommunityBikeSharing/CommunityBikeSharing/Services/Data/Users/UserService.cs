#nullable enable
using System;
using System.Threading.Tasks;
using CommunityBikeSharing.Models;

namespace CommunityBikeSharing.Services.Data.Users
{
	public class UserService : IUserService
	{
		private readonly IFirestoreContext _context;
		private readonly IUserRepository _userRepository;
		private readonly IUserEmailRepository _userEmailRepository;

		public UserService(IFirestoreContext context,
			IUserRepository userRepository,
			IUserEmailRepository userEmailRepository)
		{
			_context = context;
			_userRepository = userRepository;
			_userEmailRepository = userEmailRepository;
		}

		public async Task<User> Add(string authId, string email)
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
	}
}
