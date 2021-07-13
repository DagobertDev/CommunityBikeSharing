using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
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

		public IObservable<ICollection<Bike>> ObserveBikesFromStation(Station station)
			=> ObservableBikes(Bikes(station.CommunityId)
					.WhereEqualsTo(FieldPath.GetMappingName<Bike>(nameof(Bike.StationId)), station.Id),
				station.CommunityId);

		public async Task<IList<Bike>> GetBikesFromStation(Station station)
		{
			var result =  await Bikes(station.CommunityId).GetAsync();
			return result.ToObjects<Bike>().Select(bike =>
			{
				bike.CommunityId = station.CommunityId;
				return bike;
			}).ToList();
		}

		public IObservable<Bike?> Observe(string community, string bike)
		{
			return Bikes(community).Document(bike).AsObservable()
				.Select(snapshot =>
				{
					var b = snapshot.ToObject<Bike>();

					if (b is not null)
					{
						b.CommunityId = community;
					}

					return b;
				})
				.Catch(Observable.Return<Bike?>(null));
		}

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
		
		private static IObservable<ICollection<Bike>> ObservableBikes(IQuery query, string community)
		{
			return query.AsObservable().Select(snapshot => snapshot.ToObjects<Bike>()
					.Select(bike => 
					{ 
						bike.CommunityId = community; 
						return bike; 
					})
					.ToList())
				.Catch<ICollection<Bike>>(Observable.Return(Array.Empty<Bike>()));
		}

		protected override IDocumentReference GetDocument(Bike bike) => Bikes(bike.CommunityId).Document(bike.Id);
		protected override ICollectionReference GetCollection(Bike bike) => Bikes(bike.CommunityId);
	}
}
