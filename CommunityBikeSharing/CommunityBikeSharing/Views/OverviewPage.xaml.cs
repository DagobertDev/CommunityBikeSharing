#nullable enable
using System;
using System.Linq;
using CommunityBikeSharing.Models;
using CommunityBikeSharing.Services;
using CommunityBikeSharing.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using Xamarin.Essentials;
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
				ViewModel.OnLocationChanged += LocationChanged;
				return;
			}

			BindingContext = Startup.ServiceProvider.GetService<OverviewViewModel>();

			ViewModel.OnLocationChanged += LocationChanged;

			await ViewModel.InitializeAsync();
		}

		protected override void OnDisappearing()
		{
			ViewModel.OnLocationChanged -= LocationChanged;
		}

		private void LocationChanged(Location location)
		{
			Map.MoveToRegion(MapSpan.FromCenterAndRadius(
				location.ToPosition(),
				Distance.FromKilometers(1)));
		}

		private void OnStationSelected(object sender, EventArgs e)
		{
			ViewModel.OnStationSelected((Station)((BindableObject)sender).BindingContext);
		}

		private void OnBikeSelected(object sender, EventArgs e)
		{
			ViewModel.OnBikeSelected((Bike)((BindableObject)sender).BindingContext);
		}

		private void OnItemSelected(object sender, SelectionChangedEventArgs e)
		{
			switch (e.CurrentSelection.SingleOrDefault())
			{
				case Bike bike:
					ViewModel.OnBikeSelected(bike);
					break;
				case Station station:
					ViewModel.OnStationSelected(station);
					break;
				default:
					return;
			}

			((SelectableItemsView)sender).SelectedItem = null;
		}
	}
}

