using System;
using CommunityBikeSharing.ViewModels;
using Xamarin.Forms;

namespace CommunityBikeSharing.Views
{
	[QueryProperty(nameof(Text), "param")]
	public partial class LoadingPage
	{
		private string _text = string.Empty;
		public string Text
		{
			get => _text;
			set
			{
				_text = Uri.UnescapeDataString(value);
				OnPropertyChanged();
			}
		}

		public LoadingPage()
		{
			InitializeComponent();
		}

		protected override async void OnAppearing()
		{
			BindingContext = App.GetViewModel<LoadingViewModel>(Text);
			
			await ((BaseViewModel)BindingContext).InitializeAsync();
		}

		protected override bool OnBackButtonPressed()
		{
			// Do nothing, user shouldn't manually go back
			return true;
		}
	}
}

