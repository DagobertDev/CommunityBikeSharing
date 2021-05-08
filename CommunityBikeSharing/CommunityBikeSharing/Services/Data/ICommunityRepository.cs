using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CommunityBikeSharing.Models;

namespace CommunityBikeSharing.Services.Data
{
	public interface ICommunityRepository : IRepository<Community>
	{
		Task<Community> Create(string name, User creator);
		IObservable<Community> Observe(string id);
		Task Delete(string id);
		Task<ObservableCollection<Community>> GetCommunities(ObservableCollection<CommunityMembership> memberships);
	}
}
