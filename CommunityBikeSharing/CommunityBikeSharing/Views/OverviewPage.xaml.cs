using System;
using CommunityBikeSharing.Models;
using CommunityBikeSharing.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using Xamarin.Forms.Maps;

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

			var viewModel = Startup.ServiceProvider.GetService<OverviewViewModel>();
			BindingContext = viewModel;

			await viewModel.InitializeAsync();

			var position = new Position(viewModel.UserLocation.Latitude, viewModel.UserLocation.Longitude);
			Map.MoveToRegion(new MapSpan(position, 0.1, 0.1));
		}

		private void OnBikeSelected(object sender, PinClickedEventArgs e)
		{
			((OverviewViewModel)BindingContext).SelectedBike = (Bike)((Pin)sender).BindingContext;
		}
	}
}

