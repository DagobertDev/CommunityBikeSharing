using System;
using System.Linq;
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

		// The binding for Pin.Position is not working, so we have to set it in code-behind
		private void OnMapClicked(object sender, MapClickedEventArgs e)
		{
			((MapModalViewModel)BindingContext).SelectedLocation = e.Position.ToLocation();
			Pin.Position = e.Position;
		}

		private async void OnSearchButtonClicked(object sender, EventArgs e)
		{
			var searchText = SearchField.Text;

			if (string.IsNullOrEmpty(searchText))
			{
				return;
			}

			var positions = await Geocoder.GetPositionsForAddressAsyncFunc(searchText);
			var position = positions.FirstOrDefault();

			((MapModalViewModel)BindingContext).SelectedLocation = position.ToLocation();
			Pin.Position = position;
			Map.MoveToRegion(MapSpan.FromCenterAndRadius(position, Distance.FromKilometers(1)));
		}
	}
}

