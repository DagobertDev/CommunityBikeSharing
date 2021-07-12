using CommunityBikeSharing.ViewModels;

namespace CommunityBikeSharing.Views
{
	public partial class StartupPage
	{
		public StartupPage()
		{
			InitializeComponent();

			BindingContext = App.GetViewModel<StartupViewModel>();
		}

		protected override async void OnAppearing()
		{
			await ((BaseViewModel)BindingContext).InitializeAsync();
		}
	}
}

