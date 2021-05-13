using System;
using System.Collections.Generic;
using CommunityBikeSharing.Models;

namespace CommunityBikeSharing.Services.Data
{
	public interface IStationRepository : IRepository<Station>
	{
		public IObservable<IList<Station>> ObserveStationsFromCommunity(string communityId);
	}
}
