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
				AfterLogin = () => Shell.Current.GoToAsync("///main")
			};
		}

		private void RedirectToRegistration(object sender, EventArgs args)
		{
			Shell.Current.GoToAsync("///registration");
		}
	}
}

