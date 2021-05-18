#nullable enable
using System;
using CommunityBikeSharing.Models;

namespace CommunityBikeSharing.Services.Data.Communities
{
	public interface ICommunityRepository : IRepository<Community>
	{
		IObservable<Community> Observe(string id);
	}
}
