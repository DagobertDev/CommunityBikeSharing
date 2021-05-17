#nullable enable
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CommunityBikeSharing.Models;

namespace CommunityBikeSharing.Services.Data
{
	public interface IStationRepository : IRepository<Station>
	{
		public IObservable<IList<Station>> ObserveStationsFromCommunity(string communityId);
		public Task<IList<Station>> GetStationsFromCommunity(string communityId);
		public Task<Station> Get(string community, string id);
	}
}
