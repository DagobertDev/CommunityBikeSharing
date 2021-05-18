#nullable enable
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CommunityBikeSharing.Models;

namespace CommunityBikeSharing.Services.Data.Stations
{
	public interface IStationService
	{
		public IObservable<IList<Station>> ObserveStationsFromCommunity(string communityId);
		public ObservableCollection<Station> GetAvailableStations();
		public Task<Station> Get(string community, string id);
		Task Add(Station station);
		Task Update(Station station);
		Task Delete(Station station);
	}
}
