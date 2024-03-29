﻿using System;
using CommunityBikeSharing.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using Xamarin.Forms;

namespace CommunityBikeSharing.Views
{
	[QueryProperty(nameof(CommunityId), "param")]
	[QueryProperty(nameof(StationId), "param2")]
	public partial class EditStationPage
	{
		public EditStationPage()
		{
			InitializeComponent();
		}

		protected override async void OnAppearing()
		{
			if (BindingContext != null)
			{
				return;
			}

			BindingContext = App.GetViewModel<EditStationViewModel>(CommunityId, StationId);

			await ((BaseViewModel)BindingContext).InitializeAsync();
		}

		private string _communityId = string.Empty;
		public string CommunityId
		{
			get => _communityId;
			set
			{
				_communityId = Uri.UnescapeDataString(value);
				OnPropertyChanged();
			}
		}

		private string _stationId = string.Empty;
		public string StationId
		{
			get => _stationId;
			set
			{
				_stationId = Uri.UnescapeDataString(value);
				OnPropertyChanged();
			}
		}
	}
}

