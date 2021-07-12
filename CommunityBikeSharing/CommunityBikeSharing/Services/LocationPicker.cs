using System.Threading.Tasks;
using CommunityBikeSharing.ViewModels;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace CommunityBikeSharing.Services
{
	public class LocationPicker : ILocationPicker
	{
		private readonly INavigationService _navigationService;
		private readonly IMessagingCenter _messagingCenter;

		public LocationPicker(
			INavigationService navigationService,
			IMessagingCenter messagingCenter)
		{
			_navigationService = navigationService;
			_messagingCenter = messagingCenter;
		}

		public async Task<Location?> PickLocation()
		{
			var taskCompletionSource = new TaskCompletionSource<Location?>();

			_messagingCenter.Subscribe<MapModalViewModel, Location?>(this, nameof(Location),
				(_, location) =>
				{
					taskCompletionSource.SetResult(location);
					_messagingCenter.Unsubscribe<MapModalViewModel, Location>(this, nameof(Location));
				});

			await _navigationService.NavigateTo<MapModalViewModel>();

			return await taskCompletionSource.Task;
		}
	}
}
