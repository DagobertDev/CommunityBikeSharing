using CommunityBikeSharing.Licenses;
using CommunityBikeSharing.ViewModels;
using Xamarin.Forms;

namespace CommunityBikeSharing.Views
{
	public partial class LicensesPage
	{
		public LicensesPage()
		{
			InitializeComponent();

			BindingContext = App.GetViewModel<LicensesViewModel>();
		}

		private async void OnLicenseSelected(object sender, ItemTappedEventArgs args)
		{
			await ((LicensesViewModel)BindingContext).OnLicenseSelected((License)args.Item);
		}
	}
}

