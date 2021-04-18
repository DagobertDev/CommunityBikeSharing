using System.Threading.Tasks;
using System.Windows.Input;
using CommunityBikeSharing.Services;
using Firebase.Database;
using Firebase.Database.Query;
using Xamarin.Forms;

namespace CommunityBikeSharing.ViewModels
{
	public class MainPageViewModel : BaseViewModel
	{
		private readonly IAuthService _authService;
		private readonly FirebaseClient _firebaseClient;

		private readonly Message _message;

		private string _welcomeMessage;

		public MainPageViewModel()
		{
			_authService = DependencyService.Get<IAuthService>();
			_firebaseClient = DependencyService.Get<FirebaseClient>();
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
			EditableText = (await _firebaseClient.Child("users")
					.Child(_authService.User.Id)
					.OnceSingleAsync<Message>())?.Text;
		}

		private async void Save()
		{
			await _firebaseClient.Child("users").Child(_authService.User.Id).PutAsync(_message);
		}

		private class Message
		{
			public string Text { set; get; }
		}
	}
}
