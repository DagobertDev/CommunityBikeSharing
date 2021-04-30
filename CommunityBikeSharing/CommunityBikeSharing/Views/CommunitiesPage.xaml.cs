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
			await ((BaseViewModel)BindingContext).InitializeAsync();
		}

		private void OnCommunitySelected(object sender, ItemTappedEventArgs e)
		{
			var community = (Community)e.Item;
			((CommunitiesViewModel)BindingContext).OpenCommunityDetailCommand.Execute(community);
		}
	}
}

