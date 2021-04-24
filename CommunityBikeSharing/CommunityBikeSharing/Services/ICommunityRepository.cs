using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CommunityBikeSharing.Models;

namespace CommunityBikeSharing.Services
{
	public interface ICommunityRepository
	{
		ObservableCollection<Community> ObserveCommunities();
		Task<Community> GetCommunity(string id);
		Task AddCommunity(Community community);
	}
}
