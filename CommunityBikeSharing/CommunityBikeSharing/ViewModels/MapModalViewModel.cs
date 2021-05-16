#nullable enable
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

		public MapModalViewModel(INavigationService navigationService)
		{
			_navigationService = navigationService;
		}

		private Location? _selectedLocation;
		public Location? SelectedLocation
		{
			get => _selectedLocation;
			set
			{
				_selectedLocation = value;
				OnPropertyChanged();
			}
		}
		public ICommand ConfirmCommand => new Command(Confirm);
		private async void Confirm()
		{
			await Close(SelectedLocation);
		}

		public ICommand CancelCommand => new Command(Cancel);
		private async void Cancel()
		{
			await Close(null);
		}

		private Task Close(Location? result)
		{
			MessagingCenter.Send(this, nameof(Location), result);
			return _navigationService.NavigateBack();
		}
	}
}
