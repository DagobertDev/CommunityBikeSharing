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

		private async void OnCommunitySelected(object sender, ItemTappedEventArgs e)
		{
			var id = ((Community)e.Item).Id;

			await Shell.Current.GoToAsync($"{nameof(CommunityOverviewPage)}?CommunityId={id}");
		}
	}
}

