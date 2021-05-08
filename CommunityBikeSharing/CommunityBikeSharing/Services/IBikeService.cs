using System.Collections.ObjectModel;
using CommunityBikeSharing.Models;

namespace CommunityBikeSharing.Services
{
	public interface IBikeService
	{
		ObservableCollection<Bike> GetAvailableBikes();
	}
}
