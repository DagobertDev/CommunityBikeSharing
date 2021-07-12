using CommunityBikeSharing.Models;
using CommunityBikeSharing.ViewModels;
using Xamarin.Forms;

namespace CommunityBikeSharing.Views
{
	public partial class CommunitiesPage
	{
		public CommunitiesPage()
		{
			InitializeComponent();
		}

		protected override async void OnAppearing()
		{
			if (BindingContext != null)
			{
				return;
			}

			var viewModel = App.GetViewModel<CommunitiesViewModel>();
			BindingContext = viewModel;
			await viewModel.InitializeAsync();
		}

		private void OnCommunitySelected(object sender, ItemTappedEventArgs e)
		{
			var community = (Community)e.Item;
			((CommunitiesViewModel)BindingContext).OpenCommunityDetailCommand.Execute(community);
		}
	}
}

