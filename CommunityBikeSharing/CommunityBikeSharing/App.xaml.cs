using Plugin.CloudFirestore;
using Xamarin.Forms;

namespace CommunityBikeSharing
{
    public partial class App : Application
    {
	    public App()
        {
            InitializeComponent();

            DependencyService.RegisterSingleton(CrossCloudFirestore.Current.Instance);

            MainPage = new AppShell();
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
