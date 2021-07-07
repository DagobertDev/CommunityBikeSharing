using System.Threading.Tasks;
using CommunityBikeSharing.Models;

namespace CommunityBikeSharing.Services.Data.Users
{
	public interface IUserService
	{
		Task<User?> GetUserByEmail(string email);
		Task<bool> UpdateUsername(string name);
		Task UpdateEmail(string newMail);
		Task<User> Register(string email, string password);
		Task DeleteAccount();
	}
}
