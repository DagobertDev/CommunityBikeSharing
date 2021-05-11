using CommunityBikeSharing.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace CommunityBikeSharing.Views
{
	public partial class OverviewPage
	{
		public OverviewPage()
		{
			InitializeComponent();
		}

		protected override async void OnAppearing()
		{
			if (BindingContext != null)
			{
				return;
			}

			BindingContext = Startup.ServiceProvider.GetService<OverviewViewModel>();

			await ((BaseViewModel)BindingContext).InitializeAsync();
		}
	}
}

