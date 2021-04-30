using System;
using CommunityBikeSharing.ViewModels;
using Xamarin.Forms;

namespace CommunityBikeSharing.Views
{
	public partial class RegistrationPage : ContentPage
	{
		public RegistrationPage()
		{
			InitializeComponent();

			BindingContext = new RegistrationViewModel
			{
				AfterRegistration = () => Shell.Current.GoToAsync($"///{nameof(MainPageViewModel)}")
			};
		}

		private void RedirectToLogin(object sender, EventArgs args)
		{
			Shell.Current.GoToAsync($"///{nameof(LoginViewModel)}");
		}
	}
}

