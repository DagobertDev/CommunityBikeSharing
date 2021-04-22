using System.Collections.ObjectModel;
using CommunityBikeSharing.Models;

namespace CommunityBikeSharing.Services
{
	public interface ICommunityRepository
	{
		ObservableCollection<Community> ObserveCommunities();
		void AddCommunity(Community community);
	}
}
