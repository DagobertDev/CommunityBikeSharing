using System.Threading.Tasks;
using Xamarin.Essentials;

namespace CommunityBikeSharing.Services
{
	public interface ILocationPicker
	{
		Task<Location?> PickLocation();
	}
}
