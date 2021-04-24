using CommunityBikeSharing.ViewModels;
using Xamarin.Forms;

namespace CommunityBikeSharing.Views
{
	public partial class CommunityMembersPage : ContentPage
	{
		public CommunityMembersPage()
		{
			InitializeComponent();
		}

		protected override async void OnAppearing()
		{
			await ((CommunityMembersViewModel)BindingContext).InitializeAsync();
		}
	}
}

