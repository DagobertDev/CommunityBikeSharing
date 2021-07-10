using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CommunityBikeSharing.Models;

namespace CommunityBikeSharing.Services.Data.Bikes
{
	public interface IBikeRepository : IRepository<Bike>
	{
		public ObservableCollection<Bike> ObserveBikesFromCommunity(string communityId);
		public ObservableCollection<Bike> ObserveBikesFromStation(Station station);
		public Task<IList<Bike>> GetBikesFromStation(Station station);
		public IObservable<Bike?> Observe(string community, string bike);
	}
}
