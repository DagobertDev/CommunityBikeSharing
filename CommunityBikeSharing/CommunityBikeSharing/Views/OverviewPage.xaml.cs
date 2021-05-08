using CommunityBikeSharing.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace CommunityBikeSharing.Views
{
	public partial class OverviewPage
	{
		public OverviewPage()
		{
			InitializeComponent();

			BindingContext = Startup.ServiceProvider.GetService<OverviewViewModel>();
		}

		protected override async void OnAppearing()
		{
			await ((BaseViewModel)BindingContext).InitializeAsync();
		}
	}
}

