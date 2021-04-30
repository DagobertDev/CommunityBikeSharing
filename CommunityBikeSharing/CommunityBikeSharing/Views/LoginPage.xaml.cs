using System;
using CommunityBikeSharing.ViewModels;
using Xamarin.Forms;

namespace CommunityBikeSharing.Views
{
	public partial class LoginPage : ContentPage
	{
		public LoginPage()
		{
			InitializeComponent();

			BindingContext = new LoginViewModel
			{
				AfterLogin = () => Shell.Current.GoToAsync($"///{nameof(MainPageViewModel)}")
			};
		}

		private void RedirectToRegistration(object sender, EventArgs args)
		{
			Shell.Current.GoToAsync($"///{nameof(RegistrationViewModel)}");
		}
	}
}

