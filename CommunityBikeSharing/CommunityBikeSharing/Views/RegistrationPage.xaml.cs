using System;
using CommunityBikeSharing.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace CommunityBikeSharing.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
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

