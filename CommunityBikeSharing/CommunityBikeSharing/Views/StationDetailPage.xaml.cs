#nullable enable
using System;
using CommunityBikeSharing.Models;
using CommunityBikeSharing.ViewModels;
using Xamarin.Forms;

namespace CommunityBikeSharing.Views
{
	[QueryProperty(nameof(CommunityId), "param")]
	[QueryProperty(nameof(StationId), "param2")]
	public partial class StationDetailPage
	{
		public StationDetailPage()
		{
			InitializeComponent();
		}

		protected override async void OnAppearing()
		{
			if (BindingContext != null)
			{
				return;
			}

			var viewModel = App.GetViewModel<StationDetailViewModel>(
				StationDetailViewModel.NavigationParameters(CommunityId, StationId));
			BindingContext = viewModel;
			await viewModel.InitializeAsync();
		}

		private string _communityId = "";
		public string CommunityId
		{
			get => _communityId;
			set
			{
				_communityId = Uri.UnescapeDataString(value);
				OnPropertyChanged();
			}
		}

		private string _stationId = "";
		public string StationId
		{
			get => _stationId;
			set
			{
				_stationId = Uri.UnescapeDataString(value);
				OnPropertyChanged();
			}
		}

		private void OnBikeSelected(object sender, ItemTappedEventArgs e)
		{
			((StationDetailViewModel)BindingContext).OnBikeSelected((Bike)e.Item);
		}
	}
}

