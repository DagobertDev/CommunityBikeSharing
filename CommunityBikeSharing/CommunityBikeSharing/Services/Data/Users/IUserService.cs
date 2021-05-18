#nullable enable
using System.Threading.Tasks;
using CommunityBikeSharing.Models;

namespace CommunityBikeSharing.Services.Data.Users
{
	public interface IUserService
	{
		Task<User> Add(string authId, string email);
		Task<User?> GetUserByEmail(string email);
	}
}
