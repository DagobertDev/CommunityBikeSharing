using System;
using System.Threading.Tasks;
using CommunityBikeSharing.Models;

namespace CommunityBikeSharing.Services
{
	public interface IAuthService
	{
		Task<User> Register(string email, string password);
		Task SignIn(string email, string password);
		Task SignOut();
		Task ResetPassword(string email);
		Task UpdateUsername(string name);
		string GetCurrentUserId();
		IObservable<User> ObserveCurrentUser();
		User GetCurrentUser();
		bool SignedIn { get; }
	}
}
