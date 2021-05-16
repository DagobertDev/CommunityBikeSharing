#nullable enable
using CommunityBikeSharing.Models;
using CommunityBikeSharing.Services;
using CommunityBikeSharing.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using Xamarin.Forms;
using Xamarin.Forms.Maps;

namespace CommunityBikeSharing.Views
{
	public partial class OverviewPage
	{
		public OverviewPage()
		{
			InitializeComponent();
		}

		private OverviewViewModel ViewModel => (OverviewViewModel)BindingContext;

		protected override async void OnAppearing()
		{
			if (BindingContext != null)
			{
				return;
			}

			BindingContext = Startup.ServiceProvider.GetService<OverviewViewModel>();

			ViewModel.OnLocationChanged += location =>
			{
				Map.MoveToRegion(MapSpan.FromCenterAndRadius(
					location.ToPosition(),
					Distance.FromKilometers(1)));
			};

			await ViewModel.InitializeAsync();
		}

		private void OnStationSelected(object sender, ItemTappedEventArgs e)
		{
			ViewModel.OnStationSelected((Station)e.Item);
		}

		private void OnMapStationSelected(object sender, PinClickedEventArgs e)
		{
			ViewModel.OnStationSelected((Station)((Pin)sender).BindingContext);
		}

		private void OnBikeSelected(object sender, ItemTappedEventArgs e)
		{
			ViewModel.OnBikeSelected((Bike)e.Item);
		}

		private void OnMapBikeSelected(object sender, PinClickedEventArgs e)
		{
			ViewModel.OnBikeSelected((Bike)((Pin)sender).BindingContext);
		}
	}
}

