using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;
using CommunityBikeSharing.Models;
using CommunityBikeSharing.Services;
using Xamarin.Forms;

namespace CommunityBikeSharing.ViewModels
{
	public class LoginViewModel : BaseViewModel
	{
		private readonly IAuthService _authService;
		private readonly IDialogService _dialogService;
		private readonly INavigationService _navigationService;

		private readonly IReadOnlyDictionary<AuthError.AuthErrorReason, string> _errorMessages =
			new Dictionary<AuthError.AuthErrorReason, string>
			{
				{AuthError.AuthErrorReason.MissingPassword, "Bitte Passwort eingeben."},
				{AuthError.AuthErrorReason.MissingEmail, "Bitte Email eingeben."},
				{AuthError.AuthErrorReason.UnknownEmailAddress, "Email ist nicht registriert."},
				{AuthError.AuthErrorReason.InvalidEmailAddress, "Email ist nicht registriert."},
				{AuthError.AuthErrorReason.WrongPassword, "Passwort ist falsch."},
				{AuthError.AuthErrorReason.Undefined, "Unbekannter Fehler."}
			};

		private string _email;

		private string _password;

		public LoginViewModel(IAuthService authService,
			IDialogService dialogService,
			INavigationService navigationService)
		{
			_authService = authService;
			_dialogService = dialogService;
			_navigationService = navigationService;
		}

		public string Email
		{
			get => _email;
			set
			{
				_email = value;
				OnPropertyChanged();
			}
		}

		public string Password
		{
			get => _password;
			set
			{
				_password = value;
				OnPropertyChanged();
			}
		}

		public ICommand GoToRegistrationCommand => new Command(GoToRegistration);
		private async void GoToRegistration() => await _navigationService.NavigateToRoot<RegistrationViewModel>();

		public ICommand LoginCommand => new Command(Login);
		public ICommand ResetPasswordCommand => new Command(ResetPassword);

		private async void Login()
		{
			try
			{
				await _authService.SignIn(Email, Password);

				await _navigationService.NavigateToRoot<OverviewViewModel>();
			}
			catch (AuthError e)
			{
				await ShowLoginError(e.Reason);
			}
		}

		private async void ResetPassword()
		{
			try
			{
				await _authService.ResetPassword(_email);

				await _dialogService.ShowMessage("Passwort zurückgesetzt",
					$"Eine Email zum Zurücksetzen des Passwortes wurde an {Email} gesendet.", "Ok");
			}
			catch (AuthError e)
			{
				await _dialogService.ShowError("Fehler", GetErrorMessage(e.Reason), "Ok");
			}
		}

		private Task ShowLoginError(AuthError.AuthErrorReason reason) =>
			_dialogService.ShowError("Login fehlgeschlagen", GetErrorMessage(reason), "Ok");

		private string GetErrorMessage(AuthError.AuthErrorReason reason)
		{
			if (!_errorMessages.TryGetValue(reason, out var errorMessage))
			{
				_errorMessages.TryGetValue(AuthError.AuthErrorReason.Undefined, out errorMessage);
			}

			return errorMessage;
		}
	}
}
