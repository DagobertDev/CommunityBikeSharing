using System;
using CommunityBikeSharing.Services;
using CommunityBikeSharing.ViewModels;
using Xamarin.Forms;

namespace CommunityBikeSharing.Views
{
    public partial class MainPage : ContentPage
    {
	    private readonly IAuthService _authService;

        public MainPage()
        {
            InitializeComponent();

            BindingContext = new MainPageViewModel();

            _authService = DependencyService.Get<IAuthService>();
        }

        private async void Logout(object sender, EventArgs e)
        {
	        await _authService.SignOut();

	        await Shell.Current.GoToAsync("///login");
        }

        protected override async void OnAppearing()
        {
	        await ((MainPageViewModel)BindingContext).InitializeAsync();
        }
    }
}
