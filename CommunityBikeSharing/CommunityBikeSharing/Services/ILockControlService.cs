using System.Threading.Tasks;
using CommunityBikeSharing.Models;

namespace CommunityBikeSharing.Services
{
	public interface ILockControlService
	{
		Task<bool> OpenLock(Lock @lock);
		Task<bool> CloseLock(Lock @lock);
	}
}
