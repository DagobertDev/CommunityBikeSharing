using System.Threading.Tasks;
using System.Windows.Input;
using CommunityBikeSharing.Services;
using Xamarin.Forms;

namespace CommunityBikeSharing.ViewModels
{
	public class MainPageViewModel : BaseViewModel
	{
		private readonly IUserService _userService;
		private readonly IAuthService _authService;
		private readonly INavigationService _navigationService;

		private string _welcomeMessage;

		public MainPageViewModel(IUserService userService, INavigationService navigationService, IAuthService authService)
		{
			_userService = userService;
			_navigationService = navigationService;
			_authService = authService;
		}

		public string WelcomeMessage
		{
			get => _welcomeMessage;
			private set
			{
				_welcomeMessage = value;
				OnPropertyChanged();
			}
		}
		public override async Task InitializeAsync()
		{
			var user = await _userService.GetCurrentUser();
			WelcomeMessage = $"Hallo {user.Username}.";
		}

		public ICommand SignOutCommand => new Command(SignOut);

		private async void SignOut()
		{
			await _authService.SignOut();

			await _navigationService.NavigateToRoot<RegistrationViewModel>();
		}
	}
}
