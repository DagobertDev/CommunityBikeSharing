using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;
using CommunityBikeSharing.Services;
using Firebase.Auth;
using Xamarin.Forms;

namespace CommunityBikeSharing.ViewModels
{
	public class RegistrationViewModel : BaseViewModel
	{
		private readonly IFirebaseAuthProvider _authProvider;
		private readonly IDialogService _dialogService;

		private readonly IReadOnlyDictionary<AuthErrorReason, string> _errorMessages =
			new Dictionary<AuthErrorReason, string>
			{
				{AuthErrorReason.MissingPassword, "Bitte Passwort eingeben."},
				{AuthErrorReason.MissingEmail, "Bitte Email eingeben."},
				{AuthErrorReason.InvalidEmailAddress, "Email ist ungültig."},
				{AuthErrorReason.EmailExists, "Die Email wird bereits verwendet."},
				{AuthErrorReason.WeakPassword, "Das Passwort muss mindestens 6 Zeichen umfassen."},
				{AuthErrorReason.Undefined, "Unbekannter Fehler."}
			};

		private string _email;

		private string _password;

		private string _repeatedPassword;

		public RegistrationViewModel()
		{
			RegisterCommand = new Command(Register);
			_authProvider = DependencyService.Get<IFirebaseAuthProvider>();
			_dialogService = DependencyService.Get<IDialogService>();
		}

		public Action AfterRegistration { get; set; }

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

		public string RepeatedPassword
		{
			get => _repeatedPassword;
			set
			{
				_repeatedPassword = value;
				OnPropertyChanged();
			}
		}

		public ICommand RegisterCommand { get; }

		private async void Register()
		{
			if (string.IsNullOrEmpty(Email))
			{
				await ShowRegistrationError(AuthErrorReason.MissingEmail);
				return;
			}

			if (string.IsNullOrEmpty(Password))
			{
				await ShowRegistrationError(AuthErrorReason.MissingPassword);
				return;
			}

			if (Password != RepeatedPassword)
			{
				await _dialogService.ShowError("Registrierung fehlgeschlagen", "Passwörter stimmen nicht überein.",
					"Ok");
			}

			try
			{
				var auth = await _authProvider.CreateUserWithEmailAndPasswordAsync(Email, Password);

				App.User = auth.User;

				AfterRegistration?.Invoke();
			}
			catch (FirebaseAuthException e)
			{
				await ShowRegistrationError(e.Reason);
			}
		}

		private Task ShowRegistrationError(AuthErrorReason reason)
		{
			if (!_errorMessages.TryGetValue(reason, out var errorMessage))
			{
				_errorMessages.TryGetValue(AuthErrorReason.Undefined, out errorMessage);
			}

			return _dialogService.ShowError("Registrierung fehlgeschlagen", errorMessage, "Ok");
		}
	}
}
