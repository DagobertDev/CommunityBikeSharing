#nullable enable
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using CommunityBikeSharing.Models;
using CommunityBikeSharing.Services.Data;

namespace CommunityBikeSharing.Services
{
	public class StationService : IStationService
	{
		private readonly IStationRepository _stationRepository;
		private readonly IMembershipRepository _membershipRepository;
		private readonly IAuthService _authService;

		private readonly IDictionary<string, ObservableCollection<Station>> _allStations =
			new Dictionary<string, ObservableCollection<Station>>();

		private readonly IDictionary<string, IDisposable> _subscriptions = new Dictionary<string, IDisposable>();

		private readonly ObservableCollection<Station> _stations = new ObservableCollection<Station>();

		private readonly NotifyCollectionChangedEventHandler _stationsChanged;

		private string? _userId;

		public StationService(IStationRepository stationRepository,
			IMembershipRepository membershipRepository,
			IAuthService authService)
		{
			_stationRepository = stationRepository;
			_membershipRepository = membershipRepository;
			_authService = authService;

			_stationsChanged = (sender, args) =>
			{
				_stations.Clear();

				foreach (var station in _allStations.Values
					.SelectMany(bikes => bikes))
				{
					_stations.Add(station);
				}
			};
		}

		public ObservableCollection<Station> GetAvailableStations()
		{
			var newUserId = _authService.GetCurrentUserId();

			if (_userId == newUserId)
			{
				return _stations;
			}

			_userId = newUserId;

			var communities = _membershipRepository.ObserveMembershipsFromUser(_userId);

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

							_subscriptions[membership.CommunityId] = subscription;

							communityStations.CollectionChanged += _stationsChanged;
							_allStations[membership.Id] = communityStations;
						}
						break;
					case NotifyCollectionChangedAction.Remove:
						foreach (CommunityMembership membership in args.OldItems)
						{
							_allStations[membership.CommunityId].CollectionChanged -= _stationsChanged;
							_subscriptions[membership.CommunityId].Dispose();
						}
						break;
					case NotifyCollectionChangedAction.Reset:
						_allStations.Clear();

						foreach (var subscription in _subscriptions.Values)
						{
							subscription.Dispose();
						}

						_subscriptions.Clear();
						break;
					case NotifyCollectionChangedAction.Move:
						break;
					case NotifyCollectionChangedAction.Replace:
						throw new NotImplementedException();
					default:
						throw new NotImplementedException();
				}
			};

			return _stations;
		}
	}
}
