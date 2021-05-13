using CommunityBikeSharing.ViewModels;
using Xamarin.Forms;

namespace CommunityBikeSharing.Views
{
	public partial class CommunityStationsPage
	{
		public CommunityStationsPage()
		{
			InitializeComponent();
		}

		protected override async void OnAppearing()
		{
			await ((BaseViewModel)BindingContext).InitializeAsync();
		}

		private void OnEditStation(object sender, ItemTappedEventArgs e)
		{
			((CommunityStationsViewModel)BindingContext).EditStationCommand.Execute(e.Item);
		}
	}
}

