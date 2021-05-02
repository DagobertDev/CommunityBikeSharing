using System.Threading.Tasks;
using CommunityBikeSharing.Models;

namespace CommunityBikeSharing.Services.Data
{
	public interface IUserRepository : IRepository<User>
	{
		Task<User> GetUserByEmail(string email);
		Task<User> GetUserById(string id);
	}
}
