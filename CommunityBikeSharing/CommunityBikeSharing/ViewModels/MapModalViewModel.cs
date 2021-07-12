using System.Threading.Tasks;
using System.Windows.Input;
using CommunityBikeSharing.Services;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace CommunityBikeSharing.ViewModels
{
	public class MapModalViewModel : BaseViewModel
	{
		private readonly INavigationService _navigationService;
		private readonly IMessagingCenter _messagingCenter;

		public MapModalViewModel(
			INavigationService navigationService,
			IMessagingCenter messagingCenter)
		{
			_navigationService = navigationService;
			_messagingCenter = messagingCenter;

			ConfirmCommand = CreateCommand(Confirm);
			CancelCommand = CreateCommand(Cancel);
		}

		public ICommand ConfirmCommand { get; }
		public ICommand CancelCommand { get; }

		private Location? _selectedLocation;
		public Location? SelectedLocation
		{
			get => _selectedLocation;
			set => SetProperty(ref _selectedLocation, value);
		}

		private Task Confirm() => Close(SelectedLocation);
		private Task Cancel() => Close(null);
		
		private Task Close(Location? result)
		{
			_messagingCenter.Send(this, nameof(Location), result);
			return _navigationService.NavigateBack();
		}
	}
}
