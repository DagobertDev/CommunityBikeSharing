using CommunityBikeSharing.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using Xamarin.Forms;

namespace CommunityBikeSharing.Views
{
	public partial class StartupPage
	{
		public StartupPage()
		{
			InitializeComponent();

			BindingContext = Startup.ServiceProvider.GetService<StartupViewModel>();
		}

		protected override async void OnAppearing()
		{
			await ((BaseViewModel)BindingContext).InitializeAsync();
		}
	}
}

