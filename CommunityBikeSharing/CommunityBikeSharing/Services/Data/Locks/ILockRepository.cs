#nullable enable
using System.Threading.Tasks;
using CommunityBikeSharing.Models;

namespace CommunityBikeSharing.Services.Data.Locks
{
	public interface ILockRepository : IRepository<Lock>
	{
		Task<Lock> Get(string community, string id);
	}
}
