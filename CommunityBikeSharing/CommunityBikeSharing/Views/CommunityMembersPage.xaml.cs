using CommunityBikeSharing.ViewModels;
using Xamarin.Forms;

namespace CommunityBikeSharing.Views
{
	public partial class CommunityMembersPage
	{
		private bool _initialized;

		public CommunityMembersPage()
		{
			InitializeComponent();
		}

		protected override async void OnAppearing()
		{
			if (!_initialized)
			{
				_initialized = true;
				await ((BaseViewModel)BindingContext).InitializeAsync();
			}
		}

		private void OnEditMembership(object sender, ItemTappedEventArgs e)
		{
			var editMembership = ((CommunityMembersViewModel)BindingContext).EditMembershipCommand;

			if (editMembership.CanExecute(e.Item))
			{
				editMembership.Execute(e.Item);
			}
		}
	}
}

