﻿using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CommunityBikeSharing.Models;
using Plugin.CloudFirestore;
using Plugin.CloudFirestore.Reactive;

namespace CommunityBikeSharing.Services.Data
{
	public class BikeRepository : IBikeRepository
	{
		private readonly IFirestoreContext _firestore;
		private ICollectionReference Bikes(string communityId) => _firestore.Bikes(communityId);

		public BikeRepository(IFirestoreContext firestoreContext)
		{
			_firestore = firestoreContext;
		}

		public async Task<Bike> Add(Bike bike)
		{
			var document = await Bikes(bike.CommunityId).AddAsync(bike);
			var result = await document.GetAsync();
			return result.ToObject<Bike>();
		}

		public ObservableCollection<Bike> ObserveBikesFromCommunity(string communityId)
		{
			var result = new ObservableCollection<Bike>();

			Bikes(communityId).AsObservable().Subscribe(snapshot =>
				{
					result.Clear();

					foreach (var bike in snapshot.ToObjects<Bike>())
					{
						bike.CommunityId = communityId;
						result.Add(bike);
					}
				},
				exception =>
				{
					result.Clear();
				});

			return result;
		}

		public ObservableCollection<Bike> ObserveBikesFromStation(Station station)
		{
			var result = new ObservableCollection<Bike>();

			Bikes(station.CommunityId).WhereEqualsTo(
				FieldPath.GetMappingName<Bike>(nameof(Bike.StationId)), station.Id)
				.AsObservable().Subscribe(snapshot =>
				{
					result.Clear();

					foreach (var bike in snapshot.ToObjects<Bike>())
					{
						bike.CommunityId = station.CommunityId;
						result.Add(bike);
					}
				},
				exception =>
				{
					result.Clear();
				});

			return result;
		}

		public Task Update(Bike bike)
		{
			return Bikes(bike.CommunityId).Document(bike.Id).UpdateAsync(bike);
		}

		public Task Delete(Bike bike)
		{
			return Bikes(bike.CommunityId).Document(bike.Id).DeleteAsync();
		}
	}
}
