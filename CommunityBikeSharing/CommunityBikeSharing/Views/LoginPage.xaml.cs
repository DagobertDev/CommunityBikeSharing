using CommunityBikeSharing.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using Xamarin.Forms;

namespace CommunityBikeSharing.Views
{
	public partial class LoginPage : ContentPage
	{
		public LoginPage()
		{
			InitializeComponent();

			BindingContext = Startup.ServiceProvider.GetService<LoginViewModel>();
		}

		protected override async void OnAppearing()
		{
			await ((BaseViewModel)BindingContext).InitializeAsync();
		}
	}
}

