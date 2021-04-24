using System.Threading.Tasks;
using CommunityBikeSharing.Models;

namespace CommunityBikeSharing.Services
{
	public interface IAuthService
	{
		Task Register(string email, string password);
		Task SignIn(string email, string password);
		Task SignOut();
		Task ResetPassword(string email);
		User User { get; }

		bool SignedIn { get; }
	}
}
