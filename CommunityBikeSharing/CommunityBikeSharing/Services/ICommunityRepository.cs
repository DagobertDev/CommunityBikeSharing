using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CommunityBikeSharing.Models;

namespace CommunityBikeSharing.Services
{
	public interface ICommunityRepository
	{
		Task<Community> CreateCommunity(string name);
		Task<Community> GetCommunity(string id);
		Task UpdateCommunity(Community community);
		Task DeleteCommunity(string id);
		Task<ObservableCollection<Community>> GetCommunities();
		IObservable<Community> GetObservableCommunity(string id);
	}
}
