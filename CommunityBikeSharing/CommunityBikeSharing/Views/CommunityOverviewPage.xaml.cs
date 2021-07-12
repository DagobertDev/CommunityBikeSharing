using System;
using CommunityBikeSharing.ViewModels;
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

			BindingContext = App.GetViewModel<CommunityOverviewViewModel>(CommunityId);
			await ((BaseViewModel)BindingContext).InitializeAsync();
		}
	}
}

