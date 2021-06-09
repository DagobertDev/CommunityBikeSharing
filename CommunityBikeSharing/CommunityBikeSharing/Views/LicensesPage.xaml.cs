using CommunityBikeSharing.Licenses;
using CommunityBikeSharing.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace CommunityBikeSharing.Views
{
	public partial class LicensesPage
	{
		public LicensesPage()
		{
			InitializeComponent();

			BindingContext = Startup.ServiceProvider.GetService<LicensesViewModel>();
		}

		private void OnLicenseSelected(object sender, ItemTappedEventArgs args)
		{
			((LicensesViewModel)BindingContext).OnLicenseSelected((License)args.Item);
		}
	}
}

