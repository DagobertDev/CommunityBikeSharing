using System.Threading.Tasks;
using CommunityBikeSharing.Models;

namespace CommunityBikeSharing.Services
{
	public class UserService : IUserService
	{
		private readonly IAuthService _authService;
		private readonly IUserRepository _userRepository;

		public UserService(IAuthService authService, IUserRepository userRepository)
		{
			_authService = authService;
			_userRepository = userRepository;
		}

		public Task<User> GetCurrentUser()
		{
			var id = _authService.GetCurrentUserId();
			return _userRepository.GetUserById(id);
		}

		public async Task<User> RegisterUser(string email, string password)
		{
			var user = await _authService.Register(email, password);
			return await _userRepository.CreateUser(user);
		}
	}
}
