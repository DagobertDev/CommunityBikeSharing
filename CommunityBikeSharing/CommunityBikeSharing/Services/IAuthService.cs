using System;
using System.Threading.Tasks;
using CommunityBikeSharing.Models;

namespace CommunityBikeSharing.Services
{
	public interface IAuthService
	{
		Task<User?> Register(string email, string password);
		Task SignIn(string email, string password);
		Task SignOut();
		Task ResetPassword(string email);
		Task ChangePassword(string newPassword);
		Task UpdateUsername(string name);
		Task UpdateEmail(string email);
		string GetCurrentUserId();
		IObservable<User?> ObserveCurrentUser();
		User GetCurrentUser();
		UserData GetCurrentUserData();
		Task DeleteCurrentUser();
		Task<bool> Reauthenticate(string email, string password);
		bool SignedIn { get; }
	}
}
