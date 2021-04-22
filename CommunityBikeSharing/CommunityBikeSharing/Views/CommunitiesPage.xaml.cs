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
	}
}

