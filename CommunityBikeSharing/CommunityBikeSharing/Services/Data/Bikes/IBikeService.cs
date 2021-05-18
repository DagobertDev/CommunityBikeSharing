#nullable enable
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CommunityBikeSharing.Models;

namespace CommunityBikeSharing.Services.Data.Bikes
{
	public interface IBikeService
	{
		Task<Bike> Add(string name, string communityId);
		Task Rename(Bike bike, string name);
		Task Delete(Bike bike);
		Task LendBike(Bike bike);
		Task ReturnBike(Bike bike);
		ObservableCollection<Bike> ObserveBikesFromCommunity(string communityId);
		ObservableCollection<Bike> ObserveBikesFromStation(Station station);
		ObservableCollection<Bike> GetAvailableBikes();
	}
}
