using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using CommunityBikeSharing.Models;
using CommunityBikeSharing.Services;
using CommunityBikeSharing.Services.Data.Communities;
using CommunityBikeSharing.Services.Data.Memberships;

namespace CommunityBikeSharing.ViewModels
{
	public class CommunityOverviewViewModel : BaseViewModel
	{
		private readonly ICommunityService _communityService;
		private readonly IMembershipService _membershipService;
		private readonly IDialogService _dialogService;
		private readonly INavigationService _navigationService;

		public CommunityOverviewViewModel(
			ICommunityService communityService,
			IMembershipService membershipService,
			IDialogService dialogService,
			INavigationService navigationService,
			string id)
		{
			_communityService = communityService;
			_membershipService = membershipService;
			_dialogService = dialogService;
			_navigationService = navigationService;


			CommunityMembersViewModel =
				App.GetViewModel<CommunityMembersViewModel>(id);

			CommunityBikesViewModel =
				App.GetViewModel<CommunityBikesViewModel>(id);

			CommunityStationsViewModel =
				App.GetViewModel<CommunityStationsViewModel>(id);

			OpenSettingsCommand = CreateCommand(OpenSettings);

			PropertyChanged += (_, args) =>
			{
				if (args.PropertyName == nameof(Community))
				{
					OnPropertyChanged(nameof(Name));
				}
			};
			
			_communityService.Observe(id).Subscribe(
				community => Community = community,
				exception => _navigationService.NavigateBack());

			_membershipService.Observe(id).Subscribe(
				membership => Membership = membership,
				exception => _navigationService.NavigateBack());
		}

		public ICommand OpenSettingsCommand { get; }
		
		private Community? _community;
		private Community? Community
		{
			get => _community;
			set => SetProperty(ref _community, value);
		}
		
		private CommunityMembership? _membership;
		private CommunityMembership? Membership
		{
			get => _membership;
			set => SetProperty(ref _membership, value);
		}

		public string Name => Community?.Name ?? string.Empty;
		public CommunityMembersViewModel CommunityMembersViewModel { get; }
		public CommunityBikesViewModel CommunityBikesViewModel { get; }
		public CommunityStationsViewModel CommunityStationsViewModel { get; }

		private async Task OpenSettings()
		{
			var commands = new[]
			{
				("Reservierungsdauer festlegen", CreateCommand(UpdateReservationDuration, CanUpdateReservationDuration)),
				("Community umbenennen", CreateCommand(RenameCommunity, CanRenameCommunity)),
				("Community verlassen", CreateCommand(LeaveCommunity, CanLeaveCommunity)),
				("Community löschen", CreateCommand(DeleteCommunity, CanDeleteCommunity))
			};

			await _dialogService.ShowActionSheet("Einstellungen", "Abbrechen", commands);
		}

		private async Task UpdateReservationDuration()
		{
			if (Community is null)
			{
				throw new NullReferenceException(nameof(Community));
			}
			
			var duration = await _dialogService.ShowTextEditor("Reservierungsdauer festlegen",
				"Wie lange sollen Reservierungen gültig sein (in Stunden)?",
				keyboard: IDialogService.KeyboardType.Numeric);

			if (string.IsNullOrEmpty(duration))
			{
				return;
			}

			await _communityService.UpdateReservationDuration(Community, TimeSpan.FromHours(double.Parse(duration)));
		}

		private bool CanUpdateReservationDuration() => Membership is {IsCommunityAdmin: true};

		private async Task RenameCommunity()
		{
			if (Community is null)
			{
				throw new NullReferenceException(nameof(Community));
			}
			
			var name = await _dialogService.ShowTextEditor("Community umbenennen",
				"Wie soll die Community heißen?");

			if (string.IsNullOrEmpty(name))
			{
				return;
			}

			await _communityService.Rename(Community, name);
		}

		private bool CanRenameCommunity() => Membership is {IsCommunityAdmin: true};

		private async Task DeleteCommunity()
		{
			if (Community is null)
			{
				throw new NullReferenceException(nameof(Community));
			}
			
			var confirmed = await _dialogService.ShowConfirmation("Community löschen",
				$"Möchten Sie die Community \"{Name}\" wirklich löschen?");

			if (!confirmed)
			{
				return;
			}

			await _communityService.Delete(Community);
			await _navigationService.NavigateBack();
		}

		private bool CanDeleteCommunity() => Membership is {IsCommunityAdmin: true};

		private async Task LeaveCommunity()
		{
			if (Community is null)
			{
				throw new NullReferenceException(nameof(Community));
			}
			
			if (Membership is null)
			{
				throw new NullReferenceException(nameof(Membership));
			}
			
			var confirmed = await _dialogService.ShowConfirmation("Community verlassen",
				$"Möchten Sie die Community \"{Name}\" wirklich verlassen?");

			if (!confirmed)
			{
				return;
			}

			var allUsers =
				await _membershipService.GetMembershipsFromCommunity(Community.Id);

			if (Membership.IsCommunityAdmin && allUsers.Count(user => user.IsCommunityAdmin) <= 1)
			{
				await _dialogService.ShowError("Fehler",
					"Der letzte Community-Admin kann die Community nicht verlassen. " +
					"Ernennen Sie einen neuen Community-Admin oder löschen Sie die Community.");
				return;
			}

			await _membershipService.Delete(Membership);
			await _navigationService.NavigateBack();
		}

		private bool CanLeaveCommunity() => true;
	}
}
