using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CommunityBikeSharing.Models;

namespace CommunityBikeSharing.Services.Data
{
	public interface ICommunityRepository : IRepository<Community>
	{
		Task<Community> CreateCommunity(string name);
		IObservable<Community> GetCommunity(string id);
		Task Delete(string id);
		Task<ObservableCollection<Community>> GetCommunities(ObservableCollection<CommunityMembership> memberships);
		IObservable<Community> GetObservableCommunity(string id);
	}
}
