using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;
using CommunityBikeSharing.Models;
using CommunityBikeSharing.Services;

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

		public LoginViewModel(IAuthService authService,
			IDialogService dialogService,
			INavigationService navigationService)
		{
			_authService = authService;
			_dialogService = dialogService;
			_navigationService = navigationService;

			GoToRegistrationCommand = CreateCommand(GoToRegistration);
			LoginCommand = CreateCommand(Login);
			ResetPasswordCommand = CreateCommand(ResetPassword);
		}

		public ICommand GoToRegistrationCommand { get; }
		public ICommand LoginCommand { get; }
		public ICommand ResetPasswordCommand { get; }

		private string _email = string.Empty;
		public string Email
		{
			get => _email;
			set => SetProperty(ref _email, value);
		}

		private string _password = string.Empty;
		public string Password
		{
			get => _password;
			set => SetProperty(ref _password, value);
		}

		private async Task GoToRegistration() => await _navigationService.NavigateToRoot<RegistrationViewModel>();

		private async Task Login()
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

		private async Task ResetPassword()
		{
			try
			{
				await _authService.ResetPassword(_email);

				await _dialogService.ShowMessage("Passwort zurückgesetzt",
					$"Eine Email zum Zurücksetzen des Passwortes wurde an {Email} gesendet.");
			}
			catch (AuthError e)
			{
				await _dialogService.ShowError("Fehler", GetErrorMessage(e.Reason));
			}
		}

		private Task ShowLoginError(AuthError.AuthErrorReason reason) =>
			_dialogService.ShowError("Login fehlgeschlagen", GetErrorMessage(reason));

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
