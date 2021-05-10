using System;
using System.Threading.Tasks;
using Xamarin.Essentials;

namespace CommunityBikeSharing.Services
{
	public class LocationService : ILocationService
	{
		public async Task<Location> GetCurrentLocation()
		{
			try
			{
				var request = new GeolocationRequest(GeolocationAccuracy.Medium, TimeSpan.FromSeconds(10));
				var location = await Geolocation.GetLocationAsync(request);
				return location.IsFromMockProvider ? null : location;
			}
			catch (FeatureNotSupportedException)
			{
				// Handle not supported on device exception
			}
			catch (FeatureNotEnabledException)
			{
				// Handle not enabled on device exception
			}
			catch (PermissionException)
			{
				// Handle permission exception
			}
			catch (Exception)
			{
				// Unable to get location
			}

			return null;
		}
	}
}
