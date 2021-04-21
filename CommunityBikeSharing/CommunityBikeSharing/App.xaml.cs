using CommunityBikeSharing.Services;
using CommunityBikeSharing.Views;
using Xamarin.Forms;

namespace CommunityBikeSharing
{
    public partial class App : Application
    {
	    public App()
        {
            InitializeComponent();

            MainPage = DependencyService.Get<IAuthService>().SignedIn ? (Page)new MainPage() : new RegistrationPage();
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
