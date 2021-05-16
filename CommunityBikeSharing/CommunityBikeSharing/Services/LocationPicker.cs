#nullable enable
using System.Threading.Tasks;
using CommunityBikeSharing.ViewModels;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace CommunityBikeSharing.Services
{
	public class LocationPicker : ILocationPicker
	{
		private readonly INavigationService _navigationService;

		public LocationPicker(INavigationService navigationService)
		{
			_navigationService = navigationService;
		}

		public async Task<Location?> PickLocation()
		{
			var taskCompletionSource = new TaskCompletionSource<Location?>();

			MessagingCenter.Subscribe<MapModalViewModel, Location?>(this, nameof(Location),
				(sender, location) =>
				{
					taskCompletionSource.SetResult(location);
					MessagingCenter.Unsubscribe<MapModalViewModel, Location>(this, nameof(Location));
				});

			await _navigationService.NavigateTo<MapModalViewModel>();

			return await taskCompletionSource.Task;
		}
	}
}
