#nullable enable
using CommunityBikeSharing.Services;
using CommunityBikeSharing.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using Xamarin.Forms.Maps;

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

			var location = ((MapModalViewModel)BindingContext).SelectedLocation;

			if (location != null)
			{
				Pin.Position = location.ToPosition();
			}
		}

		private void OnMapClicked(object sender, MapClickedEventArgs e)
		{
			((MapModalViewModel)BindingContext).SelectedLocation = e.Position.ToLocation();
			Pin.Position = e.Position;
		}
	}
}

