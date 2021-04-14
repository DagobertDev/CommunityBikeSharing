using Firebase.Auth;
using Xamarin.Forms;

namespace CommunityBikeSharing
{
    public partial class App : Application
    {
	    public static User User;

	    private const string FirebaseApiKey = "AIzaSyAgXY1X3_FvHFinrAFkTKvpL-wo052R1i0";

        public App()
        {
            InitializeComponent();

            DependencyService.RegisterSingleton<IFirebaseAuthProvider>(new FirebaseAuthProvider(new FirebaseConfig(FirebaseApiKey)));

            MainPage = new RegistrationPage();
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
