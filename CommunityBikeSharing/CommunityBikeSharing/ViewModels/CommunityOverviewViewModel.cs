using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using CommunityBikeSharing.Models;
using CommunityBikeSharing.Services;
using CommunityBikeSharing.Services.Data;
using CommunityBikeSharing.Services.Data.Communities;
using CommunityBikeSharing.Services.Data.Memberships;
using Microsoft.Extensions.DependencyInjection;
using Xamarin.Forms;

namespace CommunityBikeSharing.ViewModels
{
	public class CommunityOverviewViewModel : BaseViewModel
	{
		private readonly string _id;
		private readonly ICommunityService _communityService;
		private readonly IMembershipService _membershipService;
		private readonly IDialogService _dialogService;
		private readonly INavigationService _navigationService;
		private readonly IAuthService _authService;

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

		public CommunityMembersViewModel CommunityMembersViewModel { get; }
		public CommunityBikesViewModel CommunityBikesViewModel { get; }
		public CommunityStationsViewModel CommunityStationsViewModel { get; }

		public CommunityOverviewViewModel(
			ICommunityService communityService,
			IMembershipService membershipService,
			IDialogService dialogService,
			INavigationService navigationService,
			IAuthService authService,
			string id)
		{
			_id = id;
			_communityService = communityService;
			_membershipService = membershipService;
			_dialogService = dialogService;
			_navigationService = navigationService;
			_authService = authService;

			CommunityMembersViewModel =
				ActivatorUtilities.CreateInstance<CommunityMembersViewModel>(Startup.ServiceProvider, _id);

			CommunityBikesViewModel =
				ActivatorUtilities.CreateInstance<CommunityBikesViewModel>(Startup.ServiceProvider, _id);

			CommunityStationsViewModel =
				ActivatorUtilities.CreateInstance<CommunityStationsViewModel>(Startup.ServiceProvider, _id);
		}

		public override async Task InitializeAsync()
		{
			_communityService.Observe(_id).Subscribe(
				community => Community = community,
				exception => _navigationService.NavigateBack());

			_membershipService.Observe(_id).Subscribe(
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

			await _communityService.Rename(Community, name);
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

			await _communityService.Delete(Community);
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

			var allUsers =
				await _membershipService.GetMembershipsFromCommunity(_community.Id);

			if (_membership.IsCommunityAdmin && allUsers.Count(user => user.IsCommunityAdmin) <= 1)
			{
				await _dialogService.ShowError("Fehler",
					"Der letzte Community-Admin kann die Community nicht verlassen. " +
					"Ernennen Sie einen neuen Community-Admin oder löschen Sie die Community.");
				return;
			}

			await _membershipService.Delete(_membership);
			await _navigationService.NavigateBack();
		}

		private bool CanLeaveCommunity() => true;
	}
}
