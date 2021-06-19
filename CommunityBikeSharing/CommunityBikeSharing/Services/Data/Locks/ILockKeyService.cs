using System.Threading.Tasks;
using CommunityBikeSharing.Models;

namespace CommunityBikeSharing.Services.Data.Locks
{
	public interface ILockKeyService
	{
		Task<LockKey> Get(Lock @lock);
	}
}
