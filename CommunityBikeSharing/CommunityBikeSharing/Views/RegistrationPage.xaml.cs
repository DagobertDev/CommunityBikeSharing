using CommunityBikeSharing.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace CommunityBikeSharing.Views
{
	public partial class RegistrationPage
	{
		public RegistrationPage()
		{
			InitializeComponent();

			BindingContext = Startup.ServiceProvider.GetService<RegistrationViewModel>();
		}

		protected override async void OnAppearing()
		{
			await ((BaseViewModel)BindingContext).InitializeAsync();
		}
	}
}

