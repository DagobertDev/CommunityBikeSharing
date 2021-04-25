using System.Threading.Tasks;
using CommunityBikeSharing.Models;
using CommunityBikeSharing.Services;
using Xamarin.Forms;

[assembly: Dependency(typeof(UserService))]

namespace CommunityBikeSharing.Services
{
	public class UserService : IUserService
	{
		private readonly IAuthService _authService;
		private readonly IUserRepository _userRepository;

		public UserService()
		{
			_authService = DependencyService.Get<IAuthService>();
			_userRepository = DependencyService.Get<IUserRepository>();
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
