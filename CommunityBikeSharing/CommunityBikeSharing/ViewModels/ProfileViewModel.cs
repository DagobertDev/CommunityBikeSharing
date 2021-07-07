using System;
using System.Threading.Tasks;
using System.Windows.Input;
using CommunityBikeSharing.Models;
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

		private string _username = string.Empty;
		private string _email = string.Empty;

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
			ChangeEmailCommand = new Command(async () => await ChangeEmail());
			ShowLicensesCommand = new Command(ShowLicenses);
			DeleteAccountCommand = new Command(async () => await DeleteAccount());
			ChangePasswordCommand = new Command(async () => await ChangePassword());
		}

		public string Username
		{
			get => _username;
			private set
			{
				_username = value;
				OnPropertyChanged();
			}
		}

		public string Email
		{
			get => _email;
			private set
			{
				_email = value;
				OnPropertyChanged();
			}
		}

		private bool _initialized;
		public override Task InitializeAsync()
		{
			if (_initialized)
			{
				return Task.CompletedTask;
			}

			_initialized = true;
			
			_authService.ObserveCurrentUser().Subscribe(user => Username = user?.Username ?? string.Empty);
			
			Email = _authService.GetCurrentUserData().Email;
			
			return Task.CompletedTask;
		}

		public ICommand SignOutCommand { get; }
		public ICommand ChangeUsernameCommand { get; }
		public ICommand ChangeEmailCommand { get; }
		public ICommand ShowLicensesCommand { get; }
		public ICommand DeleteAccountCommand { get; }
		public ICommand ChangePasswordCommand { get; }

		private async void SignOut()
		{
			await _authService.SignOut();

			await _navigationService.NavigateToRoot<RegistrationViewModel>();
		}

		private async Task DeleteAccount()
		{
			var confirmed = await _dialogService.ShowConfirmation("Account löschen", 
				"Möchten Sie Ihren Account wirklich löschen? Diese Aktion kann nicht rückgängig gemacht werden.");

			if (!confirmed)
			{
				return;
			}

			var password = await _dialogService.ShowTextEditor("Passwort eingeben", "Bitte geben Sie Ihr Passwort ein");

			if (string.IsNullOrEmpty(password))
			{
				return;
			}

			if (!await _authService.Reauthenticate(Email, password))
			{
				await _dialogService.ShowError("Löschen fehlgeschlagen", "Ihr Account konnte nicht gelöscht werden");
				return;
			}
			
			await _userService.DeleteAccount();

			await _navigationService.NavigateToRoot<LoginViewModel>();
		}

		private async void ChangeUserName()
		{
			var name = await _dialogService.ShowTextEditor("Benutzername eingeben",
				"Bitte geben Sie Ihren neuen Benutzernamen ein.");

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

		private async Task ChangeEmail()
		{
			var password = await _dialogService.ShowTextEditor("Passwort eingeben", "Bitte geben Sie Ihr Passwort ein");

			if (string.IsNullOrEmpty(password))
			{
				return;
			}

			if (!await _authService.Reauthenticate(Email, password))
			{
				await _dialogService.ShowError("Verifizierung fehlgeschlagen", "Ihr Account konnte nicht verifiziert werden");
				return;
			}
			
			var email = await _dialogService.ShowTextEditor("Email eingeben",
				"Bitte geben Sie Ihre neue Email-Adresse ein:", keyboard: IDialogService.KeyboardType.Email);

			if (string.IsNullOrEmpty(email))
			{
				return;
			}

			try
			{
				await _userService.UpdateEmail(email);
			}
			catch (AuthError e)
			{
				switch (e.Reason)
				{
					case AuthError.AuthErrorReason.InvalidEmailAddress:
						await _dialogService.ShowError("Email-Adresse ungültig", 
							"Die angegebene Email-Adresse ist nicht gültig.");
						return;
					case AuthError.AuthErrorReason.EmailAlreadyUsed:
						await _dialogService.ShowError("Email-Adresse bereits verwendet", 
							"Die angegebene Email-Adresse wird bereits verwendet.");
						return;
					default:
						await _dialogService.ShowError("Unbekannter Fehler", 
							"Die Email-Adresse konnte aus unbekannten Gründen nicht geändert werden.");
						return;
				}
			}
			
			Email = email;
			
			await _dialogService.ShowMessage("Email-Adresse geändert", 
				$"Ihre Email-Adresse wurde zu \"{email}\" geändert");
		}

		private async Task ChangePassword()
		{
			var oldPassword = await _dialogService.ShowTextEditor("Aktuelles Passwort eingeben", 
				"Bitte geben Sie Ihr aktuelles Passwort ein:");

			if (string.IsNullOrEmpty(oldPassword))
			{
				return;
			}

			if (!await _authService.Reauthenticate(Email, oldPassword))
			{
				await _dialogService.ShowError("Überprüfung fehlgeschlagen", "Das eingegebene Passwort ist ungültig.");
				return;
			}
			
			var newPassword = await _dialogService.ShowTextEditor("Neues Passwort eingeben",
				"Bitte geben Sie Ihr neues Passwort ein:");

			if (string.IsNullOrEmpty(newPassword))
			{
				return;
			}

			try
			{
				await _authService.ChangePassword(newPassword);
				await _dialogService.ShowMessage("Passwort geändert", "Das Passwort wurde erfolgreich geändert");
			}
			catch (AuthError e)
			{
				switch (e.Reason)
				{
					case AuthError.AuthErrorReason.WeakPassword:
						await _dialogService.ShowMessage("Passwort zu schwach", 
							"Das Passwort muss aus mindestens 6 Zeichen bestehen");
						break;
					default:
						await _dialogService.ShowMessage("Unbekannter Fehler", 
							"Das Passwort konnte aus unbekannten Gründen nicht geändert werden.");
						break;
				}
			}
		}

		private async void ShowLicenses()
		{
			await _navigationService.NavigateTo<LicensesViewModel>();
		}
	}
}
