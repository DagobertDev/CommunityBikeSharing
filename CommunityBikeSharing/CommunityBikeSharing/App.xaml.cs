using CommunityBikeSharing.Services;
using CommunityBikeSharing.Views;
using Firebase.Database;
using Xamarin.Forms;

namespace CommunityBikeSharing
{
    public partial class App : Application
    {
	    private const string FirebaseApiUrl = "https://bike-b33e3-default-rtdb.europe-west1.firebasedatabase.app/";

        public App()
        {
            InitializeComponent();

            DependencyService.RegisterSingleton(
	            new FirebaseClient(FirebaseApiUrl,
	            new FirebaseOptions
	            {
		            AuthTokenAsyncFactory = () => DependencyService.Get<IAuthService>().GetAccessToken()
	            }));

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
