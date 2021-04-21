using System.Threading.Tasks;
using System.Windows.Input;
using CommunityBikeSharing.Models;
using CommunityBikeSharing.Services;
using Xamarin.Forms;

namespace CommunityBikeSharing.ViewModels
{
	public class MainPageViewModel : BaseViewModel
	{
		private readonly IAuthService _authService;
		private readonly IMessageService _messageService;

		private readonly Message _message;

		private string _welcomeMessage;

		public MainPageViewModel()
		{
			_authService = DependencyService.Get<IAuthService>();
			_messageService = DependencyService.Get<MessageService>();
			_message = new Message();

			SaveCommand = new Command(Save);
		}

		public string EditableText
		{
			get => _message.Text;
			set
			{
				_message.Text = value;
				OnPropertyChanged();
			}
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

		public ICommand SaveCommand { get; }

		public async Task InitializeAsync()
		{
			WelcomeMessage = $"Hallo {_authService.User.Username}.";
			EditableText = (await _messageService.GetMessage()).Text;
		}

		private async void Save()
		{
			await _messageService.SaveMessage(_message);
		}
	}
}
