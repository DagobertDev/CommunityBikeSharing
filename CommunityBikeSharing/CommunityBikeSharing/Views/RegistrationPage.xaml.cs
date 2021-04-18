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
				AfterRegistration = () => Application.Current.MainPage = new MainPage()
			};
		}

		private void RedirectToLogin(object sender, EventArgs args)
		{
			Application.Current.MainPage = new LoginPage();
		}
	}
}

