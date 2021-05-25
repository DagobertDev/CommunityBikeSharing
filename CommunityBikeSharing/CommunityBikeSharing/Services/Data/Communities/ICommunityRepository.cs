#nullable enable
using System;
using CommunityBikeSharing.Models;
using Plugin.CloudFirestore;

namespace CommunityBikeSharing.Services.Data.Communities
{
	public interface ICommunityRepository : IRepository<Community>
	{
		IObservable<Community> Observe(string id);
		Community Get(string id, ITransaction transaction);
	}
}
