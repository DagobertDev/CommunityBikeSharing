using CommunityBikeSharing.Models;
using CommunityBikeSharing.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using Xamarin.Forms;

namespace CommunityBikeSharing.Views
{
	public partial class CommunitiesPage
	{
		public CommunitiesPage()
		{
			InitializeComponent();

			BindingContext = Startup.ServiceProvider.GetService<CommunitiesViewModel>();
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

