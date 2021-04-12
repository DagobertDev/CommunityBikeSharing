using System;
using Xamarin.Forms;

namespace CommunityBikeSharing
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();

            Label.Text = $"Hallo {App.User.Email}.";
        }

        private void Logout(object sender, EventArgs e)
        {
	        App.User = null;

	        Application.Current.MainPage = new RegistrationPage();
        }
    }
}
