﻿using System;
using CommunityBikeSharing.ViewModels;
using Xamarin.Forms;

namespace CommunityBikeSharing.Views
{
	[QueryProperty(nameof(License), "param")]
	public partial class LicenseDetailPage
	{
		public LicenseDetailPage()
		{
			InitializeComponent();
		}

		protected override async void OnAppearing()
		{
			if (BindingContext != null)
			{
				return;
			}

			BindingContext = App.GetViewModel<LicenseDetailViewModel>(License);
			await ((BaseViewModel)BindingContext).InitializeAsync();
		}

		private string _license = string.Empty;
		public string License
		{
			get => _license;
			set
			{
				_license = Uri.UnescapeDataString(value);
				OnPropertyChanged();
			}
		}
	}
}

