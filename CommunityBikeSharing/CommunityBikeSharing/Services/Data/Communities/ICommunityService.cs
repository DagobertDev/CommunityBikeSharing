#nullable enable
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CommunityBikeSharing.Models;

namespace CommunityBikeSharing.Services.Data.Communities
{
	public interface ICommunityService
	{
		Task<Community> Create(string name, bool showCurrentUser);
		Task Rename(Community community, string name);
		Task UpdateReservationDuration(Community community, TimeSpan reservationTime);
		Task Delete(Community community);
		IObservable<Community> Observe(string id);
		ObservableCollection<Community> GetCommunities();
	}
}
