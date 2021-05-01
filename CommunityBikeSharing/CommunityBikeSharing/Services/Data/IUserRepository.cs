using System.Threading.Tasks;
using CommunityBikeSharing.Models;

namespace CommunityBikeSharing.Services.Data
{
	public interface IUserRepository
	{
		Task<User> GetUserByEmail(string email);
		Task<User> GetUserById(string id);
		Task<User> CreateUser(User user);
	}
}
