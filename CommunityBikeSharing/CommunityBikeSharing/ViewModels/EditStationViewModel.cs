#nullable enable
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using System.Windows.Input;
using CommunityBikeSharing.Models;
using CommunityBikeSharing.Services;
using CommunityBikeSharing.Services.Data;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace CommunityBikeSharing.ViewModels
{
	public class EditStationViewModel : BaseViewModel
	{
		private readonly INavigationService _navigationService;
		private readonly IStationRepository _stationRepository;
		private readonly ILocationPicker _locationPicker;
		private readonly IDialogService _dialogService;

		public EditStationViewModel(
			INavigationService navigationService,
			IStationRepository stationRepository,
			ILocationPicker locationPicker,
			IDialogService dialogService,
			string communityId,
			string? stationId = null)
		{
			_navigationService = navigationService;
			_stationRepository = stationRepository;
			_locationPicker = locationPicker;
			_dialogService = dialogService;
			CommunityId = communityId;
			StationId = stationId;
		}

		public override async Task InitializeAsync()
		{
			if (AlreadyExists)
			{
				Station = await _stationRepository.Get(CommunityId, StationId!);
			}
			else
			{
				Station.CommunityId = CommunityId;
			}
		}

		private string CommunityId { get; }
		private string? StationId { get; }

		private Station _station = new Station();
		private Station Station
		{
			get => _station;
			set
			{
				_station = value;
				OnPropertyChanged();
				OnPropertyChanged(nameof(Name));
				OnPropertyChanged(nameof(Description));
				OnPropertyChanged(nameof(Location));
			}
		}

		[AllowNull]
		public string Name
		{
			get => _station.Name;
			set
			{
				Station.Name = value ?? string.Empty;
				OnPropertyChanged();
			}
		}

		public string? Description
		{
			get => Station.Description;
			set
			{
				Station.Description = value;
				OnPropertyChanged();
			}
		}

		public Location? Location
		{
			get => Station.Location;
			set
			{
				Station.Location = value;
				OnPropertyChanged();
			}
		}

		public bool AlreadyExists => !string.IsNullOrEmpty(StationId);

		public ICommand PickLocationCommand => new Command(PickLocation);
		private async void PickLocation()
		{
			Location = await _locationPicker.PickLocation();
		}

		public ICommand ConfirmCommand => new Command(Confirm);
		private async void Confirm()
		{
			await Task.WhenAll(
				AlreadyExists ? _stationRepository.Update(Station) : _stationRepository.Add(Station),
				_navigationService.NavigateBack());
		}

		public ICommand CancelCommand => new Command(Cancel);
		private async void Cancel()
		{
			await _navigationService.NavigateBack();
		}

		public ICommand DeleteCommand => new Command(Delete);
		public bool DeleteVisible => AlreadyExists;
		private async void Delete()
		{
			var confirmed = await _dialogService.ShowConfirmation("Station wirklich löschen?",
				$"Möchten Sie die Station \"{Name}\" wirklich löschen?");

			if (!confirmed)
			{
				return;
			}

			await Task.WhenAll(
				_stationRepository.Delete(Station),
				_navigationService.NavigateBack());
		}
	}
}
