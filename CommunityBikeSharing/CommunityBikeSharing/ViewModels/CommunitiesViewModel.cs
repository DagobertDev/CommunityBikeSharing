using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using CommunityBikeSharing.Models;
using CommunityBikeSharing.Services;
using CommunityBikeSharing.Services.Data.Communities;

namespace CommunityBikeSharing.ViewModels
{
	public class CommunitiesViewModel : BaseViewModel
	{
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

			AddCommunityCommand = CreateCommand(AddCommunity);
			OpenCommunityDetailCommand = CreateCommand<Community>(OpenCommunityOverview);
			
			Communities = _communityService.GetCommunities();
		}

		public ICommand AddCommunityCommand { get; }
		public ICommand OpenCommunityDetailCommand { get; }

		public ObservableCollection<Community> Communities { get; }

		private async Task AddCommunity()
		{
			var name = await _dialogService.ShowTextEditor("Name der Community",
				"Bitte geben Sie den Namen der neuen Community ein");

			if (string.IsNullOrEmpty(name))
			{
				return;
			}
			
			var supportEmail = await _dialogService.ShowTextEditor("Support-Email",
				"Bitte geben Sie eine Email-Adresse ein, an welche sich Mitglieder der Community bei Problemen wenden können, "
				+ "etwa wenn ein Fahrrad repariert werden muss:", keyboard: IDialogService.KeyboardType.Email);

			if (string.IsNullOrEmpty(supportEmail))
			{
				return;
			}

			var showCurrentUser = await _dialogService.ShowConfirmation("Aktuellen Ausleiher anzeigen",
				"Sollen Community-Administratoren sehen können, von wem ein Fahrrad zur Zeit ausgeliehen wird?",
				"Ja", "Nein");

			var community = await _communityService.Create(name, showCurrentUser, supportEmail);
			await OpenCommunityOverview(community);
		}

		private Task OpenCommunityOverview(Community community)
			=> _navigationService.NavigateTo<CommunityOverviewViewModel>(community.Id);
	}
}
