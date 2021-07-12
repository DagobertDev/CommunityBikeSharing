using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;
using CommunityBikeSharing.Configuration;
using CommunityBikeSharing.Models;
using CommunityBikeSharing.Services;
using CommunityBikeSharing.Services.Data.Users;

namespace CommunityBikeSharing.ViewModels
{
	public class RegistrationViewModel : BaseViewModel
	{
		private readonly IUserService _userService;
		private readonly IDialogService _dialogService;
		private readonly INavigationService _navigationService;

		private readonly IReadOnlyDictionary<AuthError.AuthErrorReason, string> _errorMessages =
			new Dictionary<AuthError.AuthErrorReason, string>
			{
				{AuthError.AuthErrorReason.MissingPassword, "Bitte Passwort eingeben."},
				{AuthError.AuthErrorReason.MissingEmail, "Bitte Email eingeben."},
				{AuthError.AuthErrorReason.InvalidEmailAddress, "Email ist ungültig."},
				{AuthError.AuthErrorReason.EmailAlreadyUsed, "Die Email wird bereits verwendet."},
				{AuthError.AuthErrorReason.WeakPassword, "Das Passwort muss mindestens 6 Zeichen umfassen."},
				{AuthError.AuthErrorReason.Undefined, "Unbekannter Fehler."}
			};

		public RegistrationViewModel(IUserService userService,
			IDialogService dialogService,
			INavigationService navigationService,
			AppSettings appSettings)
		{
			_userService = userService;
			_dialogService = dialogService;
			_navigationService = navigationService;

			ToSUrl = appSettings.ToSUrl;
			PrivacyStatementUrl = appSettings.PrivacyStatementUrl;

			GoToLoginCommand = CreateCommand(GoToLogin);
			RegisterCommand = CreateCommand(Register);
		}
		
		public ICommand GoToLoginCommand { get; }
		public ICommand RegisterCommand { get; }

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

		private string _repeatedPassword = string.Empty;
		public string RepeatedPassword
		{
			get => _repeatedPassword;
			set => SetProperty(ref _repeatedPassword, value);
		}

		public string ToSUrl { get; }
		public string PrivacyStatementUrl { get; }

		private Task GoToLogin() => _navigationService.NavigateToRoot<LoginViewModel>();

		private async Task Register()
		{
			if (Password != RepeatedPassword)
			{
				await _dialogService.ShowError("Registrierung fehlgeschlagen", "Das Passwort wurde falsch wiederholt.");
				return;
			}

			try
			{
				await _userService.Register(Email, Password);
				await _navigationService.NavigateToRoot<OverviewViewModel>();
			}
			catch (AuthError e)
			{
				await ShowRegistrationError(e.Reason);
			}
		}

		private Task ShowRegistrationError(AuthError.AuthErrorReason reason)
		{
			if (!_errorMessages.TryGetValue(reason, out var errorMessage))
			{
				_errorMessages.TryGetValue(AuthError.AuthErrorReason.Undefined, out errorMessage);
			}

			return _dialogService.ShowError("Registrierung fehlgeschlagen", errorMessage);
		}
	}
}
