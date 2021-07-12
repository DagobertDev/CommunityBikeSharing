using CommunityBikeSharing.ViewModels;

namespace CommunityBikeSharing.Views
{
	public partial class RegistrationPage
	{
		public RegistrationPage()
		{
			InitializeComponent();

			BindingContext = App.GetViewModel<RegistrationViewModel>();
		}

		protected override async void OnAppearing()
		{
			await ((BaseViewModel)BindingContext).InitializeAsync();
		}
	}
}

