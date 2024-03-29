﻿using System.Threading.Tasks;
using CommunityBikeSharing.Services;

namespace CommunityBikeSharing.ViewModels
{
	public class StartupViewModel : BaseViewModel
	{
		private readonly IAuthService _authService;
		private readonly INavigationService _navigationService;

		public StartupViewModel(IAuthService authService, INavigationService navigationService)
		{
			_authService = authService;
			_navigationService = navigationService;
		}

		public override async Task InitializeAsync()
		{
			if (_authService.SignedIn)
			{
				await _navigationService.NavigateToRoot<OverviewViewModel>();
			}
			else
			{
				await _navigationService.NavigateToRoot<RegistrationViewModel>();
			}
		}
	}
}
