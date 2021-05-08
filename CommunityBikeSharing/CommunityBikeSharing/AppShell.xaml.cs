using CommunityBikeSharing.ViewModels;
using CommunityBikeSharing.Views;
using Xamarin.Forms;

namespace CommunityBikeSharing
{
	public partial class AppShell
	{
		public AppShell()
		{
			InitializeComponent();

			Routing.RegisterRoute(nameof(CommunityOverviewViewModel), typeof(CommunityOverviewPage));
			OverviewPage.Route = nameof(OverviewViewModel);
			Login.Route = nameof(LoginViewModel);
			Registration.Route = nameof(RegistrationViewModel);
		}
	}
}

