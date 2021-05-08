using CommunityBikeSharing.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace CommunityBikeSharing.Views
{
    public partial class ProfilePage
    {
	    public ProfilePage()
        {
            InitializeComponent();

            BindingContext = Startup.ServiceProvider.GetService<ProfileViewModel>();
        }

        protected override async void OnAppearing()
        {
	        await ((BaseViewModel)BindingContext).InitializeAsync();
        }
    }
}
