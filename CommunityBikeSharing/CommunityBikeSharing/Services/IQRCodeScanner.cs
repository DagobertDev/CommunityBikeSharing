using System.Threading.Tasks;

namespace CommunityBikeSharing.Services
{
	public interface IQRCodeScanner
	{
		Task<string?> Scan();
	}
}
