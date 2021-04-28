using System.Threading.Tasks;
using System.Windows.Input;
using CommunityBikeSharing.Models;
using CommunityBikeSharing.Services;
using Xamarin.Forms;

namespace CommunityBikeSharing.ViewModels
{
	public class CommunityOverviewViewModel : BaseViewModel
	{
		private readonly string _id;
		private readonly ICommunityRepository _communityRepository;
		private readonly IMembershipRepository _membershipRepository;
		private readonly IDialogService _dialogService;

		private Community _community;
		private CommunityMembership _membership;

		private Community Community
		{
			get => _community;
			set
			{
				_community = value;
				OnPropertyChanged();
				OnPropertyChanged(nameof(Name));
			}
		}


		public string Name
		{
			get => _community.Name;
			set
			{
				_community.Name = value;
				OnPropertyChanged();
			}
		}

		private CommunityMembersViewModel _communitiesViewModel;
		public CommunityMembersViewModel CommunityMembersViewModel
		{
			get => _communitiesViewModel;
			private set
			{
				_communitiesViewModel = value;
				OnPropertyChanged();
			}
		}

		public CommunityOverviewViewModel(string id)
		{
			_id = id;
			_communityRepository = DependencyService.Get<ICommunityRepository>();
			_membershipRepository = DependencyService.Get<IMembershipRepository>();
			_dialogService = DependencyService.Get<IDialogService>();
			CommunityMembersViewModel = new CommunityMembersViewModel(_id);
			OpenSettingsCommand = new Command(OpenSettings);
		}

		public async Task InitializeAsync()
		{
			Community = await _communityRepository.GetCommunity(_id);

			var user = await DependencyService.Get<IUserService>().GetCurrentUser();

			_membership = await _membershipRepository.Get(Community, user);
		}

		public ICommand OpenSettingsCommand { get; }

		private async void OpenSettings()
		{
			var commands = new (string, ICommand)[]
			{
				("Community umbenennen", new Command(RenameCommunity, CanRenameCommunity)),
				("Community verlassen", new Command(LeaveCommunity, CanLeaveCommunity)),
				("Community löschen", new Command(DeleteCommunity, CanDeleteCommunity))
			};

			await _dialogService.ShowActionSheet("Einstellungen", "Abbrechen", commands);
		}

		private async void RenameCommunity()
		{
			var name = await _dialogService.ShowTextEditor("Community umbenennen",
				"Wie soll die Community heißen?");

			if (string.IsNullOrEmpty(name))
			{
				return;
			}

			Name = name;
			await _communityRepository.UpdateCommunity(Community);
		}

		private bool CanRenameCommunity() => _membership.IsCommunityAdmin;

		private async void DeleteCommunity()
		{
			var confirmed = await _dialogService.ShowConfirmation("Community löschen",
				$"Möchten Sie die Community \"{Name}\" wirklich löschen?");

			if (!confirmed)
			{
				return;
			}

			await _communityRepository.DeleteCommunity(Community.Id);
			await Shell.Current.Navigation.PopAsync();
		}

		private bool CanDeleteCommunity() => _membership.IsCommunityAdmin;

		private async void LeaveCommunity()
		{
			var confirmed = await _dialogService.ShowConfirmation("Community verlassen",
				$"Möchten Sie die Community \"{Name}\" wirklich verlassen?");

			if (!confirmed)
			{
				return;
			}

			await _membershipRepository.Delete(_membership);
			await Shell.Current.Navigation.PopAsync();
		}

		private bool CanLeaveCommunity() => true;
	}
}
