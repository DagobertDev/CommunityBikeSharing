using System;
using CommunityBikeSharing.ViewModels;
using Microsoft.Extensions.DependencyInjection;
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

			BindingContext = ActivatorUtilities.CreateInstance<LicenseDetailViewModel>(
				Startup.ServiceProvider, License);
			await ((BaseViewModel)BindingContext).InitializeAsync();
		}

		private string _license = "";
		public string License
		{
			get => _license;
			set
			{
				_license = Uri.UnescapeDataString(value ?? string.Empty);
				OnPropertyChanged();
			}
		}
	}
}

