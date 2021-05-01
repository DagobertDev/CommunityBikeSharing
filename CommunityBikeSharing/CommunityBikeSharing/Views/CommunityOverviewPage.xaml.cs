﻿using System;
using CommunityBikeSharing.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using Xamarin.Forms;

namespace CommunityBikeSharing.Views
{
	[QueryProperty(nameof(CommunityId), "param")]
	public partial class CommunityOverviewPage : TabbedPage
	{
		string _communityId = "";
		public string CommunityId
		{
			get => _communityId;
			set
			{
				_communityId = Uri.UnescapeDataString(value ?? string.Empty);
				OnPropertyChanged();
			}
		}

		public CommunityOverviewPage()
		{
			InitializeComponent();
		}

		protected override async void OnAppearing()
		{
			BindingContext = ActivatorUtilities.CreateInstance<CommunityOverviewViewModel>(
				Startup.ServiceProvider, CommunityId);
			await ((BaseViewModel)BindingContext).InitializeAsync();
		}
	}
}

