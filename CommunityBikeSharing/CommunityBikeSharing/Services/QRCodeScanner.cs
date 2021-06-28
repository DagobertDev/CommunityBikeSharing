using System.Threading.Tasks;
using ZXing.Mobile;

namespace CommunityBikeSharing.Services
{
	public class QRCodeScanner : IQRCodeScanner
	{
		public async Task<string?> Scan()
		{
			var scanner = new MobileBarcodeScanner();
			var result = await scanner.Scan();
			return result?.Text;
		}
	}
}
