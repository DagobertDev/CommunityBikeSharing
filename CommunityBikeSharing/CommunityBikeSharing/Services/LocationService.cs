using System;
using System.Threading.Tasks;
using Xamarin.Essentials;

namespace CommunityBikeSharing.Services
{
	public class LocationService : ILocationService
	{
		private readonly IDialogService _dialogService;
		public LocationService(IDialogService dialogService)
		{
			_dialogService = dialogService;
		}

		public async Task<Location?> GetCurrentLocation()
		{
			try
			{
				var request = new GeolocationRequest(GeolocationAccuracy.High, TimeSpan.FromSeconds(10));
				var location = await Geolocation.GetLocationAsync(request);
				return location.IsFromMockProvider ? null : location;
			}
			catch (FeatureNotEnabledException)
			{
				await _dialogService.ShowError("Standort aktivieren",
					"Bitte aktivieren Sie die Standortfunktion des Gerätes.");
			}
			catch (PermissionException)
			{
				await _dialogService.ShowError("Standort erlauben",
					"Bitte erlauben Sie der App den Zugriff auf den Standort des Gerätes.");
			}
			catch (Exception)
			{
				await _dialogService.ShowError("Zugriff auf Standort fehlgeschlagen",
					"Es ist ein Fehler beim Zugriff auf den Standort des Gerätes aufgetreten.");
			}

			return null;
		}
	}
}
