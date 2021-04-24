using CommunityBikeSharing.Models;
using CommunityBikeSharing.ViewModels;
using Xamarin.Forms;

namespace CommunityBikeSharing.Views
{
	public partial class CommunitiesPage : ContentPage
	{
		public CommunitiesPage()
		{
			InitializeComponent();

			BindingContext = new CommunitiesViewModel();
		}

		protected override async void OnAppearing()
		{
			await ((CommunitiesViewModel)BindingContext).InitializeAsync();
		}

		private async void ListView_OnItemSelected(object sender, SelectedItemChangedEventArgs e)
		{
			var id = ((Community)e.SelectedItem).Id;

			await Shell.Current.GoToAsync($"{nameof(CommunityOverviewPage)}?CommunityId={id}");
		}
	}
}

