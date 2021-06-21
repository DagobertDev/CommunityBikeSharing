using System;
using CommunityBikeSharing.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using Xamarin.Forms;

namespace CommunityBikeSharing.Views
{
	[QueryProperty(nameof(CommunityId), "param")]
	public partial class CommunityOverviewPage
	{
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

		public CommunityOverviewPage()
		{
			InitializeComponent();
		}

		protected override async void OnAppearing()
		{
			if (BindingContext != null)
			{
				return;
			}

			BindingContext = ActivatorUtilities.CreateInstance<CommunityOverviewViewModel>(
				Startup.ServiceProvider, CommunityId);
			await ((BaseViewModel)BindingContext).InitializeAsync();
		}
	}
}

