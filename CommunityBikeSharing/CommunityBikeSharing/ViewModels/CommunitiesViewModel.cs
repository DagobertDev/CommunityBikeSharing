using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using CommunityBikeSharing.Models;
using CommunityBikeSharing.Services;
using CommunityBikeSharing.Services.Data.Communities;
using Xamarin.Forms;

namespace CommunityBikeSharing.ViewModels
{
	public class CommunitiesViewModel : BaseViewModel
	{
		private ObservableCollection<Community>? _communities;
		public ObservableCollection<Community>? Communities
		{
			get => _communities;
			set
			{
				_communities = value;
				OnPropertyChanged();
			}
		}

		private readonly ICommunityService _communityService;
		private readonly IDialogService _dialogService;
		private readonly INavigationService _navigationService;
		public CommunitiesViewModel(
			ICommunityService communityService,
			IDialogService dialogService,
			INavigationService navigationService)
		{
			_communityService = communityService;
			_dialogService = dialogService;
			_navigationService = navigationService;
		}

		public ICommand AddCommunityCommand => new Command(AddCommunity);

		private async void AddCommunity()
		{
			var name = await _dialogService.ShowTextEditor("Name der Community",
				"Bitte geben Sie den Namen der neuen Community ein");

			if (string.IsNullOrEmpty(name))
			{
				return;
			}

			var showCurrentUser = await _dialogService.ShowConfirmation("Aktuellen Ausleiher anzeigen",
				"Sollen Community-Administratoren sehen können, von wem ein Fahrrad zur Zeit ausgeliehen wird?",
				"Ja", "Nein");

			var community = await _communityService.Create(name, showCurrentUser);
			OpenCommunityDetail(community);
		}

		public ICommand OpenCommunityDetailCommand => new Command<Community>(OpenCommunityDetail);

		private async void OpenCommunityDetail(Community community)
		{
			await _navigationService.NavigateTo<CommunityOverviewViewModel>(community.Id);
		}

		public override Task InitializeAsync()
		{
			Communities = _communityService.GetCommunities();
			return Task.CompletedTask;
		}
	}
}
