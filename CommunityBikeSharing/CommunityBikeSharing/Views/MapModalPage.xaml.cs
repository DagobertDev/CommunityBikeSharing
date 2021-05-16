using CommunityBikeSharing.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace CommunityBikeSharing.Views
{
	public partial class MapModalPage
	{
		public MapModalPage()
		{
			InitializeComponent();

			BindingContext = Startup.ServiceProvider.GetService<MapModalViewModel>();
		}

		protected override async void OnAppearing()
		{
			await ((BaseViewModel)BindingContext).InitializeAsync();
		}
	}
}

