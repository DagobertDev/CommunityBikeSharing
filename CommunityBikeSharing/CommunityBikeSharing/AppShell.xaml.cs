using CommunityBikeSharing.ViewModels;
using CommunityBikeSharing.Views;
using Xamarin.Forms;

namespace CommunityBikeSharing
{
	public partial class AppShell : Shell
	{
		public AppShell()
		{
			InitializeComponent();

			Routing.RegisterRoute(nameof(CommunityOverviewViewModel), typeof(CommunityOverviewPage));
			MainPage.Route = nameof(MainPageViewModel);
			Login.Route = nameof(LoginViewModel);
			Registration.Route = nameof(RegistrationViewModel);
		}
	}
}

