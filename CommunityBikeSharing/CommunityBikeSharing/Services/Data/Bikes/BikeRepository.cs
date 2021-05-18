#nullable enable
using System;
using System.Collections.ObjectModel;
using CommunityBikeSharing.Models;
using Plugin.CloudFirestore;
using Plugin.CloudFirestore.Reactive;

namespace CommunityBikeSharing.Services.Data.Bikes
{
	public class BikeRepository : FirestoreRepository<Bike>, IBikeRepository
	{
		private readonly IFirestoreContext _context;
		private ICollectionReference Bikes(string communityId) => _context.Bikes(communityId);

		public BikeRepository(IFirestoreContext context)
		{
			_context = context;
		}

		public ObservableCollection<Bike> ObserveBikesFromCommunity(string communityId)
			=> ObserveBikes(Bikes(communityId), communityId);

		public ObservableCollection<Bike> ObserveBikesFromStation(Station station)
			=> ObserveBikes(Bikes(station.CommunityId)
					.WhereEqualsTo(FieldPath.GetMappingName<Bike>(nameof(Bike.StationId)), station.Id),
				station.CommunityId);

		private static ObservableCollection<Bike> ObserveBikes(IQuery query, string community)
		{
			var result = new ObservableCollection<Bike>();

			query.AsObservable().Subscribe(snapshot =>
					{
						result.Clear();

						foreach (var bike in snapshot.ToObjects<Bike>())
						{
							bike.CommunityId = community;
							result.Add(bike);
						}
					},
					exception =>
					{
						result.Clear();
					});

			return result;
		}

		protected override IDocumentReference GetDocument(Bike bike) => Bikes(bike.CommunityId).Document(bike.Id);
		protected override ICollectionReference GetCollection(Bike bike) => Bikes(bike.CommunityId);
	}
}
