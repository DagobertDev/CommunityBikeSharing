using CommunityBikeSharing.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using Xamarin.Forms;

namespace CommunityBikeSharing.Views
{
	public partial class RegistrationPage : ContentPage
	{
		public RegistrationPage()
		{
			InitializeComponent();

			BindingContext = Startup.ServiceProvider.GetService<RegistrationViewModel>();
		}

		protected override async void OnAppearing()
		{
			await ((BaseViewModel)BindingContext).InitializeAsync();
		}
	}
}

