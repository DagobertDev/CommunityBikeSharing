using CommunityBikeSharing.Services;
using CommunityBikeSharing.Views;
using Firebase.Auth;
using Firebase.Database;
using Xamarin.Forms;

namespace CommunityBikeSharing
{
    public partial class App : Application
    {
	    private const string FirebaseApiKey = "AIzaSyAgXY1X3_FvHFinrAFkTKvpL-wo052R1i0";

        public App()
        {
            InitializeComponent();

            DependencyService.RegisterSingleton<IFirebaseAuthProvider>(new FirebaseAuthProvider(new FirebaseConfig(FirebaseApiKey)));
            DependencyService.RegisterSingleton(
	            new FirebaseClient("https://bike-b33e3-default-rtdb.europe-west1.firebasedatabase.app/",
	            new FirebaseOptions {AuthTokenAsyncFactory = () => DependencyService.Get<IAuthService>().GetAccessToken()}));
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
