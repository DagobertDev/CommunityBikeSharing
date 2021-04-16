using System;
using CommunityBikeSharing.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace CommunityBikeSharing.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class LoginPage : ContentPage
	{
		public LoginPage()
		{
			InitializeComponent();

			BindingContext = new LoginViewModel
			{
				AfterLogin = () => Application.Current.MainPage = new MainPage()
			};
		}

		private void RedirectToRegistration(object sender, EventArgs args)
		{
			Application.Current.MainPage = new RegistrationPage();
		}
	}
}

