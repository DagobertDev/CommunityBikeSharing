#nullable enable
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks;
using CommunityBikeSharing.Models;
using CommunityBikeSharing.Services.Data.Memberships;

namespace CommunityBikeSharing.Services.Data.Stations
{
	public class StationService : IStationService
	{
		private readonly IStationRepository _stationRepository;

		public StationService(IStationRepository stationRepository,
			IMembershipService membershipService)
		{
			_stationRepository = stationRepository;

			_availableStations = new Lazy<ObservableCollection<Station>>(() =>
			{
				IDictionary<string, ObservableCollection<Station>> allStations =
					new Dictionary<string, ObservableCollection<Station>>();

				IDictionary<string, IDisposable> subscriptions = new Dictionary<string, IDisposable>();

				ObservableCollection<Station> result = new ObservableCollection<Station>();

				var communities = membershipService.ObserveMemberships();

				communities.CollectionChanged += (sender, args) =>
				{
					switch (args.Action)
					{
						case NotifyCollectionChangedAction.Add:
							foreach (CommunityMembership membership in args.NewItems)
							{
								ObservableCollection<Station> communityStations = new ObservableCollection<Station>();

								var subscription = _stationRepository
									.ObserveStationsFromCommunity(membership.CommunityId)
									.Subscribe(stations =>
									{
										communityStations.Clear();

										foreach (var station in stations)
										{
											communityStations.Add(station);
										}
									}, exception =>
									{
										communityStations.Clear();
									});

								subscriptions[membership.CommunityId] = subscription;

								communityStations.CollectionChanged += StationsChanged;
								allStations[membership.Id] = communityStations;
							}
							break;
						case NotifyCollectionChangedAction.Remove:
							foreach (CommunityMembership membership in args.OldItems)
							{
								allStations[membership.CommunityId].CollectionChanged -= StationsChanged;
								subscriptions[membership.CommunityId].Dispose();
							}
							break;
						case NotifyCollectionChangedAction.Reset:
							allStations.Clear();

							foreach (var subscription in subscriptions.Values)
							{
								subscription.Dispose();
							}

							subscriptions.Clear();
							break;
						case NotifyCollectionChangedAction.Move:
							break;
						case NotifyCollectionChangedAction.Replace:
							throw new NotImplementedException();
						default:
							throw new NotImplementedException();
					}
				};

				return result;

				void StationsChanged(object sender, NotifyCollectionChangedEventArgs args)
				{
					result.Clear();

					foreach (var station in allStations.Values.SelectMany(bikes => bikes))
					{
						result.Add(station);
					}
				}
			});
		}

		public IObservable<IList<Station>> ObserveStationsFromCommunity(string communityId) =>
			_stationRepository.ObserveStationsFromCommunity(communityId);

		private readonly Lazy<ObservableCollection<Station>> _availableStations;
		public ObservableCollection<Station> GetAvailableStations() => _availableStations.Value;

		public Task<Station> Get(string community, string id) => _stationRepository.Get(community, id);
		public Task Add(Station station) => _stationRepository.Add(station);

		public Task Update(Station station) => _stationRepository.Add(station);
		public Task Delete(Station station) => _stationRepository.Delete(station);
	}
}
