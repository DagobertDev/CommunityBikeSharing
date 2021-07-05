using System;
using System.Threading.Tasks;
using CommunityBikeSharing.Models;
using Plugin.CloudFirestore;

namespace CommunityBikeSharing.Services.Data.Communities
{
	public interface ICommunityRepository : IRepository<Community>
	{
		IObservable<Community> Observe(string id);
		Task<Community> Get(string id);
		Community Get(string id, ITransaction transaction);
	}
}
