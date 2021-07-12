using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using CommunityBikeSharing.Models;
using CommunityBikeSharing.Services;
using CommunityBikeSharing.Services.Data.Memberships;
using CommunityBikeSharing.Services.Data.Users;
using Xamarin.Essentials;

namespace CommunityBikeSharing.ViewModels
{
	public class CommunityMembersViewModel : BaseViewModel
	{
		private readonly string _communityId;
		private readonly IMembershipService _membershipService;
		private readonly IDialogService _dialogService;
		private readonly IUserService _userService;

		public CommunityMembersViewModel(
			IMembershipService membershipService,
			IDialogService dialogService,
			IUserService userService,
			string communityId)
		{
			_membershipService = membershipService;
			_dialogService = dialogService;
			_userService = userService;
			_communityId = communityId;

			AddMemberCommand = CreateCommand(AddMember);
			PromoteCommunityAdminCommand = CreateCommand<CommunityMembership>(PromoteCommunityAdmin, CanPromoteCommunityAdmin);
			RemoveMemberCommand = CreateCommand<CommunityMembership>(RemoveMember, CanRemoveMember);
			EditMembershipCommand = CreateCommand<CommunityMembership>(EditMembership);
			
			PropertyChanged += (_, args) =>
			{
				if (args.PropertyName == nameof(CurrentUserMembership))
				{
					OnPropertyChanged(nameof(AddMemberVisible));
				}
			};

			Members = _membershipService.ObserveMembershipsFromCommunity(_communityId);
			Members.CollectionChanged += (_, _) => OnPropertyChanged(nameof(SortedMembers));
			
			membershipService.Observe(_communityId).Subscribe(
				membership => CurrentUserMembership = membership,
				exception => CurrentUserMembership = null);
		}

		public ICommand AddMemberCommand { get; }
		public ICommand PromoteCommunityAdminCommand { get; }
		public ICommand RemoveMemberCommand { get; }
		public ICommand EditMembershipCommand { get; }
		
		private CommunityMembership? _currentUserMembership;
		private CommunityMembership? CurrentUserMembership
		{
			get => _currentUserMembership;
			set => SetProperty(ref _currentUserMembership, value);
		}

		public ObservableCollection<CommunityMembership> Members { get; }
		public IEnumerable<CommunityMembership> SortedMembers => 
			Members.OrderBy(m => m.Role).ThenBy(m => m.Name);

		public bool AddMemberVisible => CurrentUserMembership is {Role: CommunityRole.CommunityAdmin};

		private async Task AddMember()
		{
			var email = await _dialogService.ShowTextEditor("Nutzer hinzufügen",
				"Geben Sie die Email-Adresse des neuen Mitgliedes ein:",
				"Hinzufügen",
				keyboard: IDialogService.KeyboardType.Email);

			if (string.IsNullOrEmpty(email))
			{
				return;
			}

			var user = await _userService.GetUserByEmail(email);

			if (user == null)
			{
				var sendMail = await _dialogService.ShowConfirmation("Nutzer nicht gefunden",
					$"Es konnte kein Nutzer mit der Email \"{email}\" gefunden werden. " +
					$"Möchten Sie eine Einladung an \"{email}\" versenden?", "Mail-App öffnen");

				if (sendMail)
				{
					await SendInvitationMail(email);
				}

				return;
			}

			if (Members.Select(membership => membership.UserId).Contains(user.Id))
			{
				await _dialogService.ShowError("Benutzer bereits Mitglied",
					$"Der Benutzer \"{email}\" ist bereits Mitglied der Community.");
				return;
			}

			await _membershipService.Add(user, _communityId);
		}

		private void EditMembership(CommunityMembership membership)
		{
			var actions = new []
			{
				("Zum Community-Admin machen", PromoteCommunityAdminCommand),
				("Nutzer entfernen", RemoveMemberCommand)
			};

			_dialogService.ShowActionSheet(membership.Name, "Abbrechen", actions, membership);
		}

		private Task PromoteCommunityAdmin(CommunityMembership membership)
			=> _membershipService.PromoteToCommunityAdmin(membership);

		private bool CanPromoteCommunityAdmin(CommunityMembership membership) =>
			membership.Role == CommunityRole.User && UserIsAdminAndEditsOtherUser(membership);

		private async Task RemoveMember(CommunityMembership membership)
		{
			await _membershipService.Delete(membership);
		}

		private bool CanRemoveMember(CommunityMembership membership) => UserIsAdminAndEditsOtherUser(membership);

		private bool UserIsAdminAndEditsOtherUser(CommunityMembership membership) =>
			CurrentUserMembership is {Role: CommunityRole.CommunityAdmin} && CurrentUserMembership.Id != membership.Id;

		private async Task SendInvitationMail(string mailAddress)
		{
			try
			{
				var message = new EmailMessage
				{
					Subject = "Einladung für Community-Bike Sharing",
					Body = "Hallo,\n" +
					       "um Teil der Bike-Sharing Community zu werden, " +
					       "installieren Sie bitte Community Bike-Sharing App.",
					To = new List<string>{mailAddress}
				};

				await Email.ComposeAsync(message);
			}
			catch (Exception)
			{
				await _dialogService.ShowError("Versand fehlgeschlagen",
					"Es konnte keine Mail-App auf dem Gerät gefunden werden");
			}
		}
	}
}
