using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CommunityBikeSharing.Models;

namespace CommunityBikeSharing.Services
{
	public interface IBikeService
	{
		ObservableCollection<Bike> GetAvailableBikes();
		Task LendBike(Bike bike);
		Task ReturnBike(Bike bike);
	}
}
