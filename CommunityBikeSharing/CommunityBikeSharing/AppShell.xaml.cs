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
			Routing.RegisterRoute(nameof(MapModalViewModel), typeof(MapModalPage));
			Routing.RegisterRoute(nameof(EditStationViewModel), typeof(EditStationPage));
			Routing.RegisterRoute(nameof(StationDetailViewModel), typeof(StationDetailPage));
			Routing.RegisterRoute(nameof(LicensesViewModel), typeof(LicensesPage));
			Routing.RegisterRoute(nameof(LicenseDetailViewModel), typeof(LicenseDetailPage));
			Routing.RegisterRoute(nameof(LoadingViewModel), typeof(LoadingPage));
			OverviewPage.Route = nameof(OverviewViewModel);
			Login.Route = nameof(LoginViewModel);
			Registration.Route = nameof(RegistrationViewModel);
		}
	}
}

