#nullable enable
using System.Threading.Tasks;
using Xamarin.Essentials;

namespace CommunityBikeSharing.Services
{
	public interface ILocationService
	{
		Task<Location?> GetCurrentLocation();
	}
}
