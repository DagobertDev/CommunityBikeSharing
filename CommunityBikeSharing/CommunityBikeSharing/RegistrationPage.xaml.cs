using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Firebase.Auth;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace CommunityBikeSharing
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class RegistrationPage : ContentPage
	{
		public RegistrationPage()
		{
			InitializeComponent();
		}

		private readonly IReadOnlyDictionary<AuthErrorReason, string> _errorMessages = new Dictionary<AuthErrorReason, string>()
		{
			{ AuthErrorReason.MissingPassword, "Bitte Password eingeben." },
			{ AuthErrorReason.MissingEmail, "Bitte Email eingeben." },
			{ AuthErrorReason.InvalidEmailAddress, "Email ist ungültig." },
			{ AuthErrorReason.EmailExists, "Die Email wird bereits verwendet." },
			{ AuthErrorReason.WeakPassword, "Das Password muss mindestens 6 Zeichen umfassen."},
			{ AuthErrorReason.Undefined, "Unbekannter Fehler." },
		};

		public async void Register(object sender, EventArgs args)
		{
			var email = EmailField.Text;
			var password = PasswordField.Text;

			var authProvider = new FirebaseAuthProvider(new FirebaseConfig("AIzaSyAgXY1X3_FvHFinrAFkTKvpL-wo052R1i0"));

			if (string.IsNullOrEmpty(email))
			{
				await DisplayAuthErrorNotification(AuthErrorReason.MissingEmail);
				return;
			}

			if (string.IsNullOrEmpty(password))
			{
				await DisplayAuthErrorNotification(AuthErrorReason.MissingPassword);
				return;
			}

			if (password != RepeatedPasswordField.Text)
			{
				await DisplayErrorNotification("Passwörter stimmen nicht überein.");
			}

			try
			{
				var auth = await authProvider.CreateUserWithEmailAndPasswordAsync(email, password);

				App.User = auth.User;

				Application.Current.MainPage = new MainPage();
			}
			catch (FirebaseAuthException e)
			{
				await DisplayAuthErrorNotification(e.Reason);
			}
		}

		private Task DisplayAuthErrorNotification(AuthErrorReason reason)
		{
			if (!_errorMessages.TryGetValue(reason, out var errorMessage))
			{
				_errorMessages.TryGetValue(AuthErrorReason.Undefined, out errorMessage);
			}

			return DisplayErrorNotification(errorMessage);
		}

		private Task DisplayErrorNotification(string errorMessage)
		{
			return DisplayAlert("Registrierung fehlgeschlagen", errorMessage, "Ok");
		}

		private void RedirectToLogin(object sender, EventArgs args)
		{
			Application.Current.MainPage = new LoginPage();
		}
	}
}

