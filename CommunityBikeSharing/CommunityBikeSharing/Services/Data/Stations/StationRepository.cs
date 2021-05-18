#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using CommunityBikeSharing.Models;
using Plugin.CloudFirestore;
using Plugin.CloudFirestore.Reactive;

namespace CommunityBikeSharing.Services.Data.Stations
{
	public class StationRepository : FirestoreRepository<Station>, IStationRepository
	{
		private readonly IFirestoreContext _context;
		private ICollectionReference Stations(string communityId) => _context.Stations(communityId);

		public StationRepository(IFirestoreContext context)
		{
			_context = context;
		}

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

		public async Task<IList<Station>> GetStationsFromCommunity(string communityId)
		{
			var snapshot = await Stations(communityId).GetAsync();
			return snapshot.ToObjects<Station>().ToList();
		}

		public async Task<Station> Get(string community, string id)
		{
			var snap = await Stations(community).Document(id).GetAsync();
			var station = snap.ToObject<Station>();
			station!.CommunityId = community;
			return station;
		}

		protected override IDocumentReference GetDocument(Station station) => Stations(station.CommunityId).Document(station.Id);
		protected override ICollectionReference GetCollection(Station station) => Stations(station.CommunityId);
	}
}
