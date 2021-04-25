using System.Threading.Tasks;
using CommunityBikeSharing.Services;
using Xamarin.Forms;

namespace CommunityBikeSharing.ViewModels
{
	public class MainPageViewModel : BaseViewModel
	{
		private readonly IUserService _userService;

		private string _welcomeMessage;

		public MainPageViewModel()
		{
			_userService = DependencyService.Get<IUserService>();
		}

		public string WelcomeMessage
		{
			get => _welcomeMessage;
			private set
			{
				_welcomeMessage = value;
				OnPropertyChanged();
			}
		}
		public async Task InitializeAsync()
		{
			var user = await _userService.GetCurrentUser();
			WelcomeMessage = $"Hallo {user.Username}.";
		}
	}
}
