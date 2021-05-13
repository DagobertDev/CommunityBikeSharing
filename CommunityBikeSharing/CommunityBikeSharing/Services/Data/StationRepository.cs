using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using CommunityBikeSharing.Models;
using Plugin.CloudFirestore;
using Plugin.CloudFirestore.Reactive;

namespace CommunityBikeSharing.Services.Data
{
	public class StationRepository : IStationRepository
	{
		private ICollectionReference Stations(string communityId) => _context.Stations(communityId);

		public StationRepository(IFirestoreContext context)
		{
			_context = context;
		}

		private readonly IFirestoreContext _context;

		public async Task<Station> Add(Station model)
		{
			await Stations(model.CommunityId).AddAsync(model);
			return model;
		}

		public Task Update(Station station) =>
			Stations(station.CommunityId).Document(station.CommunityId).UpdateAsync(station);

		public Task Delete(Station station) => Stations(station.CommunityId).Document(station.Id).DeleteAsync();

		public IObservable<IList<Station>> ObserveStationsFromCommunity(string communityId)
		{
			return Stations(communityId).AsObservable()
				.Select(snapshot => snapshot.ToObjects<Station>())
				.Select(stations => stations.Select(station =>
				{
					station.CommunityId = communityId;
					return station;
				}).ToList());
		}
	}
}
