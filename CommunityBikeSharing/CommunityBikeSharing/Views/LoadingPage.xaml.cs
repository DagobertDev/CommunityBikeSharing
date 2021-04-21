using CommunityBikeSharing.ViewModels;
using Xamarin.Forms;

namespace CommunityBikeSharing.Views
{
	public partial class LoadingPage : ContentPage
	{
		public LoadingPage()
		{
			InitializeComponent();
		}

		protected override async void OnAppearing()
		{
			var viewModel = new LoadingViewModel();
			await viewModel.InitializeAsync();
		}
	}
}

