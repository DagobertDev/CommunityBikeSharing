using CommunityBikeSharing.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using Xamarin.Forms;

namespace CommunityBikeSharing.Views
{
	public partial class LoadingPage : ContentPage
	{
		public LoadingPage()
		{
			InitializeComponent();

			BindingContext = Startup.ServiceProvider.GetService<LoadingViewModel>();
		}

		protected override async void OnAppearing()
		{
			await ((BaseViewModel)BindingContext).InitializeAsync();
		}
	}
}

