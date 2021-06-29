using System;
using CommunityBikeSharing.ViewModels;
using Microsoft.Extensions.DependencyInjection;
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
			BindingContext = ActivatorUtilities.CreateInstance<LoadingViewModel>(
				Startup.ServiceProvider, Text);
			
			await ((BaseViewModel)BindingContext).InitializeAsync();
		}
	}
}

