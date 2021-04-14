using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;
using CommunityBikeSharing.Services;
using Firebase.Auth;
using Xamarin.Forms;

namespace CommunityBikeSharing.ViewModels
{
	public class LoginViewModel : BaseViewModel
	{
		private readonly IFirebaseAuthProvider _authProvider;
		private readonly IDialogService _dialogService;

		private readonly IReadOnlyDictionary<AuthErrorReason, string> _errorMessages =
			new Dictionary<AuthErrorReason, string>
			{
				{AuthErrorReason.MissingPassword, "Bitte Passwort eingeben."},
				{AuthErrorReason.MissingEmail, "Bitte Email eingeben."},
				{AuthErrorReason.UnknownEmailAddress, "Email ist nicht registriert."},
				{AuthErrorReason.InvalidEmailAddress, "Email ist nicht registriert."},
				{AuthErrorReason.WrongPassword, "Passwort ist falsch."},
				{AuthErrorReason.Undefined, "Unbekannter Fehler."}
			};

		private string _email;

		private string _password;

		public LoginViewModel()
		{
			LoginCommand = new Command(Login);
			ResetPasswordCommand = new Command(ResetPassword);
			_authProvider = DependencyService.Get<IFirebaseAuthProvider>();
			_dialogService = DependencyService.Get<IDialogService>();
		}

		public Action AfterLogin { get; set; }

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

		public ICommand LoginCommand { get; }
		public ICommand ResetPasswordCommand { get; }

		private async void Login()
		{
			if (string.IsNullOrEmpty(Email))
			{
				await ShowLoginError(AuthErrorReason.MissingEmail);
				return;
			}

			if (string.IsNullOrEmpty(Password))
			{
				await ShowLoginError(AuthErrorReason.MissingPassword);
				return;
			}

			try
			{
				var auth = await _authProvider.SignInWithEmailAndPasswordAsync(Email, Password);

				App.User = auth.User;

				AfterLogin?.Invoke();
			}
			catch (FirebaseAuthException e)
			{
				await ShowLoginError(e.Reason);
			}
		}

		private async void ResetPassword()
		{
			if (string.IsNullOrEmpty(Email))
			{
				await _dialogService.ShowError("Fehler", GetErrorMessage(AuthErrorReason.MissingEmail), "Ok");
				return;
			}

			try
			{
				await _authProvider.SendPasswordResetEmailAsync(Email);

				await _dialogService.ShowMessage("Passwort zurückgesetzt",
					$"Eine Email zum Zurücksetzen des Passwortes wurde an {Email} gesendet.", "Ok");
			}
			catch (FirebaseAuthException e)
			{
				await _dialogService.ShowError("Fehler", GetErrorMessage(e.Reason), "Ok");
			}
		}

		private Task ShowLoginError(AuthErrorReason reason) =>
			_dialogService.ShowError("Login fehlgeschlagen", GetErrorMessage(reason), "Ok");

		private string GetErrorMessage(AuthErrorReason reason)
		{
			if (!_errorMessages.TryGetValue(reason, out var errorMessage))
			{
				_errorMessages.TryGetValue(AuthErrorReason.Undefined, out errorMessage);
			}

			return errorMessage;
		}
	}
}
