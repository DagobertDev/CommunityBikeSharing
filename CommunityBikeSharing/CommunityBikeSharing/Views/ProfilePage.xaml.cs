using CommunityBikeSharing.ViewModels;

namespace CommunityBikeSharing.Views
{
    public partial class ProfilePage
    {
	    public ProfilePage()
        {
            InitializeComponent();

            BindingContext = App.GetViewModel<ProfileViewModel>();
        }

        protected override async void OnAppearing()
        {
	        await ((BaseViewModel)BindingContext).InitializeAsync();
        }
    }
}
