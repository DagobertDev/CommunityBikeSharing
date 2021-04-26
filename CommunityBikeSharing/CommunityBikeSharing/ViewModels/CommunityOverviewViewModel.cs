using System;
using System.Collections.Generic;
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
		private readonly IDialogService _dialogService;

		private Community _community;
		private CommunityMember _member;

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
			_dialogService = DependencyService.Get<IDialogService>();
			CommunityMembersViewModel = new CommunityMembersViewModel(_id);
			OpenSettingsCommand = new Command(OpenSettings);
		}

		public async Task InitializeAsync()
		{
			Community = await _communityRepository.GetCommunity(_id);
			_member = await _communityRepository.GetCommunityMember(_id);
		}

		public ICommand OpenSettingsCommand { get; }

		private async void OpenSettings()
		{
			var actions = new List<(string, Action)>();

			if (_member.Role == CommunityRole.CommunityAdmin)
			{
				actions.Add(("Community umbenennen", RenameCommunity));
			}

			actions.Add(("Community verlassen", LeaveCommunity));

			if (_member.Role == CommunityRole.CommunityAdmin)
			{
				actions.Add(("Community löschen", DeleteCommunity));
			}

			await _dialogService.ShowActionSheet("Einstellungen", "Abbrechen", actions.ToArray());
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

		private async void DeleteCommunity()
		{
			var confirm = await _dialogService.ShowConfirmation("Community löschen",
				$"Möchten Sie die Community \"{Name}\" wirklich löschen?");

			if (confirm)
			{
				await _communityRepository.DeleteCommunity(Community.Id);
			}
		}

		private async void LeaveCommunity()
		{
			var confirm = await _dialogService.ShowConfirmation("Community verlassen",
				$"Möchten Sie die Community \"{Name}\" wirklich verlassen?");

			// TODO: Allow leaving communities
		}
	}
}
