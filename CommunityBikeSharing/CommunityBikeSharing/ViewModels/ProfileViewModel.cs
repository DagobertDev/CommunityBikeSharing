﻿using System;
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

		private string _welcomeMessage = string.Empty;

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
			DeleteAccountCommand = new Command(async () => await DeleteAccount());
			ChangePasswordCommand = new Command(async () => await ChangePassword());
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

		private bool _initialized;
		public override Task InitializeAsync()
		{
			if (_initialized)
			{
				return Task.CompletedTask;
			}

			_initialized = true;
			
			_authService.ObserveCurrentUser().Subscribe(user => WelcomeMessage = $"Benutzername: {user?.Username}");
			return Task.CompletedTask;
		}

		public ICommand SignOutCommand { get; }
		public ICommand ChangeUsernameCommand { get; }
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

			var userData = _authService.GetCurrentUserData();
			var password = await _dialogService.ShowTextEditor("Passwort eingeben", "Bitte geben Sie Ihr Passwort ein");

			if (!await _authService.Reauthenticate(userData.Email, password))
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

		private async Task ChangePassword()
		{
			var userData = _authService.GetCurrentUserData();
			var oldPassword = await _dialogService.ShowTextEditor("Aktuelles Passwort eingeben", 
				"Bitte geben Sie Ihr aktuelles Passwort ein:");

			if (!await _authService.Reauthenticate(userData.Email, oldPassword))
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
							"Das Passwort konnte wegen unbekannten Gründen nicht geändert werden.");
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
