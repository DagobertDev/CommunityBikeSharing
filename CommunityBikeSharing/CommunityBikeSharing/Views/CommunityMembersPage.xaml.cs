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
			await ((BaseViewModel)BindingContext).InitializeAsync();
		}

		private void OnEditMembership(object sender, ItemTappedEventArgs e)
		{
			((CommunityMembersViewModel)BindingContext).EditMembershipCommand.Execute(e.Item);
		}
	}
}

