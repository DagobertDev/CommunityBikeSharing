#nullable enable
using System.Threading.Tasks;
using CommunityBikeSharing.Models;

namespace CommunityBikeSharing.Services.Data.Users
{
	public interface IUserEmailRepository : IRepository<UserEmail>
	{
		public Task<UserEmail?> Get(string email);
	}
}
