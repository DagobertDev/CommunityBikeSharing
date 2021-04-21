using System.Threading.Tasks;
using CommunityBikeSharing.Services;
using Xamarin.Forms;

namespace CommunityBikeSharing.ViewModels
{
	public class LoadingViewModel : BaseViewModel
	{
		private readonly IAuthService _authService;

		public LoadingViewModel()
		{
			_authService = DependencyService.Get<IAuthService>();
		}

		public async Task InitializeAsync()
		{
			if (_authService.SignedIn)
			{
				await Shell.Current.GoToAsync("///main");
			}
			else
			{
				await Shell.Current.GoToAsync("///registration");
			}
		}
	}
}
