#nullable enable
using System.Threading.Tasks;
using CommunityBikeSharing.Models;

namespace CommunityBikeSharing.Services.Data.Users
{
	public interface IUserRepository : IRepository<User>
	{
		Task<User> Get(string id);
	}
}
