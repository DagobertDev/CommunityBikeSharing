using CommunityBikeSharing.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using Xamarin.Forms;

namespace CommunityBikeSharing.Views
{
    public partial class MainPage : ContentPage
    {
	    public MainPage()
        {
            InitializeComponent();

            BindingContext = Startup.ServiceProvider.GetService<MainPageViewModel>();
        }

        protected override async void OnAppearing()
        {
	        await ((BaseViewModel)BindingContext).InitializeAsync();
        }
    }
}
