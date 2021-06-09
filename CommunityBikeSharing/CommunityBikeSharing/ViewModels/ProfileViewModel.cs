using System;
using System.Threading.Tasks;
using System.Windows.Input;
using CommunityBikeSharing.Services;
using CommunityBikeSharing.Services.Data.Users;
using Xamarin.Forms;

namespace CommunityBikeSharing.ViewModels
{
	public class ProfileViewModel : BaseViewModel
	{
		private readonly IAuthService _authService;
		private readonly INavigationService _navigationService;
		private readonly IDialogService _dialogService;
		private readonly IUserService _userService;

		private string _welcomeMessage;

		public ProfileViewModel(
			INavigationService navigationService,
			IAuthService authService,
			IDialogService dialogService,
			IUserService userService)
		{
			_navigationService = navigationService;
			_authService = authService;
			_dialogService = dialogService;
			_userService = userService;

			SignOutCommand = new Command(SignOut);
			ChangeUsernameCommand = new Command(ChangeUserName);
			ShowLicensesCommand = new Command(ShowLicenses);
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

		public ICommand SignOutCommand { get; }
		public ICommand ChangeUsernameCommand { get; }
		public ICommand ShowLicensesCommand { get; }

		private async void SignOut()
		{
			await _authService.SignOut();

			await _navigationService.NavigateToRoot<RegistrationViewModel>();
		}

		private async void ChangeUserName()
		{
			var name = await _dialogService.ShowTextEditor("Benutzername eingeben",
				"Bitte geben Sie ihren neuen Benutzernamen ein.");

			if (string.IsNullOrEmpty(name))
			{
				return;
			}

			if (!await _userService.UpdateUsername(name))
			{
				await _dialogService.ShowError("Änderung fehlgeschlagen",
					"Der gewählte Benutzername ist bereits vergeben.");
			}
		}

		private async void ShowLicenses()
		{
			await _navigationService.NavigateTo<LicensesViewModel>();
		}
	}
}
