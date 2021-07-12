using CommunityBikeSharing.ViewModels;

namespace CommunityBikeSharing.Views
{
	public partial class LoginPage
	{
		public LoginPage()
		{
			InitializeComponent();

			BindingContext = App.GetViewModel<LoginViewModel>();
		}

		protected override async void OnAppearing()
		{
			await ((BaseViewModel)BindingContext).InitializeAsync();
		}
	}
}

