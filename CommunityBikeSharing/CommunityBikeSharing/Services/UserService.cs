﻿using System.Threading.Tasks;
using CommunityBikeSharing.Models;
using CommunityBikeSharing.Services.Data;

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
	}
}
