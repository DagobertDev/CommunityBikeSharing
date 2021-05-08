using System;
using System.Threading.Tasks;
using System.Windows.Input;
using CommunityBikeSharing.Services;
using Xamarin.Forms;

namespace CommunityBikeSharing.ViewModels
{
	public class ProfileViewModel : BaseViewModel
	{
		private readonly IAuthService _authService;
		private readonly INavigationService _navigationService;

		private string _welcomeMessage;

		public ProfileViewModel(INavigationService navigationService, IAuthService authService)
		{
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
		public override Task InitializeAsync()
		{
			_authService.ObserveCurrentUser().Subscribe(user => WelcomeMessage = $"Benutzername: {user?.Username}");
			return Task.CompletedTask;
		}

		public ICommand SignOutCommand => new Command(SignOut);

		private async void SignOut()
		{
			await _authService.SignOut();

			await _navigationService.NavigateToRoot<RegistrationViewModel>();
		}
	}
}
