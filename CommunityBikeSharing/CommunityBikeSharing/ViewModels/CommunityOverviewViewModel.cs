using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using CommunityBikeSharing.Models;
using CommunityBikeSharing.Services;
using CommunityBikeSharing.Services.Data;
using Microsoft.Extensions.DependencyInjection;
using Xamarin.Forms;

namespace CommunityBikeSharing.ViewModels
{
	public class CommunityOverviewViewModel : BaseViewModel
	{
		private readonly string _id;
		private readonly ICommunityRepository _communityRepository;
		private readonly IMembershipRepository _membershipRepository;
		private readonly IDialogService _dialogService;
		private readonly INavigationService _navigationService;
		private readonly IUserService _userService;

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

		private CommunityMembersViewModel _communityMembersViewModel;
		public CommunityMembersViewModel CommunityMembersViewModel
		{
			get => _communityMembersViewModel;
			private set
			{
				_communityMembersViewModel = value;
				OnPropertyChanged();
			}
		}

		private CommunityBikesViewModel _communityBikesViewModel;
		public CommunityBikesViewModel CommunityBikesViewModel
		{
			get => _communityBikesViewModel;
			private set
			{
				_communityBikesViewModel = value;
				OnPropertyChanged();
			}
		}

		public CommunityOverviewViewModel(
			ICommunityRepository communityRepository,
			IMembershipRepository membershipRepository,
			IDialogService dialogService,
			INavigationService navigationService,
			IUserService userService,
			string id)
		{
			_id = id;
			_communityRepository = communityRepository;
			_membershipRepository = membershipRepository;
			_dialogService = dialogService;
			_navigationService = navigationService;
			_userService = userService;

			CommunityMembersViewModel =
				ActivatorUtilities.CreateInstance<CommunityMembersViewModel>(Startup.ServiceProvider, _id);

			CommunityBikesViewModel =
				ActivatorUtilities.CreateInstance<CommunityBikesViewModel>(Startup.ServiceProvider, _id);
		}

		public override async Task InitializeAsync()
		{
			_communityRepository.GetCommunity(_id).Subscribe(
				community => Community = community,
				exception => _navigationService.NavigateBack()
				);

			var user = await _userService.GetCurrentUser();

			_membershipRepository.Get(Community, user).Subscribe(
				membership => _membership = membership,
				exception => _navigationService.NavigateBack());
		}

		public ICommand OpenSettingsCommand => new Command(OpenSettings);

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
			await _communityRepository.Update(Community);
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

			await _communityRepository.Delete(Community.Id);
			await _navigationService.NavigateBack();
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

			var allUsers = _membershipRepository.ObserveMembershipsFromCommunity(Community);

			if (_membership.IsCommunityAdmin && allUsers.Count(user => user.IsCommunityAdmin) <= 1)
			{
				await _dialogService.ShowError("Fehler",
					"Der letzte Community-Admin kann die Community nicht verlassen. " +
					"Ernennen Sie einen neuen Community-Admin oder löschen Sie die Community.");
				return;
			}

			await _membershipRepository.Delete(_membership);
			await _navigationService.NavigateBack();
		}

		private bool CanLeaveCommunity() => true;
	}
}
