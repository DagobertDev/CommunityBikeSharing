using System.Threading.Tasks;
using CommunityBikeSharing.Models;
using CommunityBikeSharing.Services;
using Plugin.CloudFirestore;
using Xamarin.Forms;

[assembly: Dependency(typeof(MessageService))]

namespace CommunityBikeSharing.Services
{
	public class MessageService : IMessageService
	{
		private readonly IAuthService _authService;

		public MessageService()
		{
			_authService = DependencyService.Get<IAuthService>();
		}

		public async Task<Message> GetMessage()
		{
			var document = await CrossCloudFirestore.Current
				.Instance
				.Collection("users")
				.Document(_authService.User.Id)
				.GetAsync();

			return document.Exists ? document.ToObject<Message>() : new Message();
		}

		public Task SaveMessage(Message message) =>
			CrossCloudFirestore.Current
				.Instance
				.Collection("users")
				.Document(_authService.User.Id)
				.SetAsync(message);
	}
}
