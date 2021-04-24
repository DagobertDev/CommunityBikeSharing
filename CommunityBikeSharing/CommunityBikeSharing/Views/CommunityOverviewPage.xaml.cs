using System;
using CommunityBikeSharing.ViewModels;
using Xamarin.Forms;

namespace CommunityBikeSharing.Views
{
	[QueryProperty(nameof(CommunityId), nameof(CommunityId))]
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
			BindingContext = new CommunityOverviewViewModel(CommunityId);
			await ((CommunityOverviewViewModel)BindingContext).InitializeAsync();
		}


		private async void OpenSettings(object sender, EventArgs e)
		{
			await Shell.Current.GoToAsync($"{nameof(CommunitySettingsPage)}?{nameof(CommunitySettingsPage.CommunityId)}={CommunityId}");
		}
	}
}

