using CommunityBikeSharing.ViewModels;
using Xamarin.Forms;

namespace CommunityBikeSharing.Views
{
    public partial class MainPage : ContentPage
    {
	    public MainPage()
        {
            InitializeComponent();

            BindingContext = new MainPageViewModel();
        }

        protected override async void OnAppearing()
        {
	        await ((BaseViewModel)BindingContext).InitializeAsync();
        }
    }
}
