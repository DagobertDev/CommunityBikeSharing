using CommunityBikeSharing.ViewModels;
using Xamarin.Forms;

namespace CommunityBikeSharing.Views
{
	public partial class CommunityBikesPage
	{
		public CommunityBikesPage()
		{
			InitializeComponent();
		}

		protected override async void OnAppearing()
		{
			await ((BaseViewModel)BindingContext).InitializeAsync();
		}

		private void OnEditBike(object sender, ItemTappedEventArgs e)
		{
			((CommunityBikesViewModel)BindingContext).EditBikeCommand.Execute(e.Item);
		}
	}
}

