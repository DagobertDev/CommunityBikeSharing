using System.Threading.Tasks;
using CommunityBikeSharing.Services;
using Xamarin.Forms;

namespace CommunityBikeSharing.ViewModels
{
	public class LoadingViewModel : BaseViewModel
	{
		private readonly IAuthService _authService;
		private readonly INavigationService _navigationService;

		public LoadingViewModel()
		{
			_authService = DependencyService.Get<IAuthService>();
			_navigationService = DependencyService.Get<INavigationService>();
		}

		public override async Task InitializeAsync()
		{
			if (_authService.SignedIn)
			{
				await _navigationService.NavigateToRoot<MainPageViewModel>();
			}
			else
			{
				await _navigationService.NavigateToRoot<RegistrationViewModel>();
			}
		}
	}
}
