using System.Threading.Tasks;
using CommunityBikeSharing.Models;

namespace CommunityBikeSharing.Services
{
	public interface IUserService
	{
		Task<User> GetCurrentUser();
	}
}
