#nullable enable
using System.Threading.Tasks;
using CommunityBikeSharing.Models;

namespace CommunityBikeSharing.Services.Data.Locks
{
	public interface ILockService
	{
		Task<Lock> Get(Bike bike);
		Task Add(Bike bike, string name, string key);
		Task Remove(Bike bike);
	}
}
