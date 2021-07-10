using System;
using CommunityBikeSharing.ViewModels;
using Xamarin.Forms;

namespace CommunityBikeSharing.Views
{
	[QueryProperty(nameof(CommunityId), "param")]
	[QueryProperty(nameof(BikeId), "param2")]
	public partial class EditBikePage
	{
		public EditBikePage()
		{
			InitializeComponent();
		}

		protected override async void OnAppearing()
		{
			if (BindingContext != null)
			{
				return;
			}

			BindingContext = App.GetViewModel<EditBikeViewModel>(CommunityId, BikeId);

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

		private string _bikeId = string.Empty;
		public string BikeId
		{
			get => _bikeId;
			set
			{
				_bikeId = Uri.UnescapeDataString(value);
				OnPropertyChanged();
			}
		}
	}
}
