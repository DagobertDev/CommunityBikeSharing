using System;
using CommunityBikeSharing.ViewModels;
using Xamarin.Forms;

namespace CommunityBikeSharing.Views
{
	[QueryProperty(nameof(CommunityId), nameof(CommunityId))]
	public partial class CommunitySettingsPage : ContentPage
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

		public CommunitySettingsPage()
		{
			InitializeComponent();
		}

		protected override async void OnAppearing()
		{
			if (BindingContext == null)
			{
				BindingContext = new CommunityOverviewViewModel(CommunityId);
			}

			await ((CommunityOverviewViewModel)BindingContext).InitializeAsync();
		}

		private async void RenameClicked(object sender, EventArgs e)
		{
			((CommunityOverviewViewModel)BindingContext).Name =
				await DisplayPromptAsync("Community umbenennen", "Wie soll die Community heißen?");
		}

		private async void LeaveClicked(object sender, EventArgs e)
		{
			await DisplayAlert("Community verlassen",
				$"Möchten Sie die Community \"{((CommunityOverviewViewModel)BindingContext).Name}\" wirklich verlassen?",
				"Ja", "Nein");
		}
	}
}
