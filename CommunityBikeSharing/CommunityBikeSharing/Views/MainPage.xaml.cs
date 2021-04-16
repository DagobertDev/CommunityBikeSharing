using System;
using CommunityBikeSharing.Services;
using Xamarin.Forms;

namespace CommunityBikeSharing.Views
{
    public partial class MainPage : ContentPage
    {
	    private readonly IAuthService _authService;

        public MainPage()
        {
            InitializeComponent();

            _authService = DependencyService.Get<IAuthService>();

            Label.Text = $"Hallo {_authService.User.Username}.";
        }

        private async void Logout(object sender, EventArgs e)
        {
	        await _authService.SignOut();

	        Application.Current.MainPage = new RegistrationPage();
        }
    }
}
