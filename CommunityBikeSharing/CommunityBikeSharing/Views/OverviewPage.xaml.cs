using System;
using System.ComponentModel;
using System.Linq;
using CommunityBikeSharing.Models;
using CommunityBikeSharing.Services;
using CommunityBikeSharing.ViewModels;
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
				ViewModel.PropertyChanged += LocationChanged;
				return;
			}

			BindingContext = App.GetViewModel<OverviewViewModel>();

			ViewModel.PropertyChanged += LocationChanged;

			await ViewModel.InitializeAsync();
		}

		protected override void OnDisappearing()
		{
			ViewModel.PropertyChanged -= LocationChanged;
		}

		private void LocationChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == nameof(OverviewViewModel.UserLocation))
			{
				Map.MoveToRegion(MapSpan.FromCenterAndRadius(
					ViewModel.UserLocation.ToPosition(),
					Distance.FromKilometers(1)));
			}
		}

		private async void OnStationSelected(object sender, EventArgs e)
		{
			await ViewModel.OnStationSelected((Station)((BindableObject)sender).BindingContext);
		}

		private async void OnBikeSelected(object sender, EventArgs e)
		{
			await ViewModel.OnBikeSelected((Bike)((BindableObject)sender).BindingContext);
		}

		private async void OnItemSelected(object sender, SelectionChangedEventArgs e)
		{
			var selectedItem = e.CurrentSelection.SingleOrDefault();
			((SelectableItemsView)sender).SelectedItem = null;

			switch (selectedItem)
			{
				case Bike bike:
					await ViewModel.OnBikeSelected(bike);
					break;
				case Station station:
					await ViewModel.OnStationSelected(station);
					break;
				default:
					return;
			}
		}
	}
}

