using System.Threading.Tasks;
using CommunityBikeSharing.Models;

namespace CommunityBikeSharing.Services
{
	public interface IMessageService
	{
		Task<Message> GetMessage();
		Task SaveMessage(Message message);
	}
}
