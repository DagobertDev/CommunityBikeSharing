using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using CommunityBikeSharing.Models;
using CommunityBikeSharing.Services;
using CommunityBikeSharing.Services.Data.Memberships;
using CommunityBikeSharing.Services.Data.Users;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace CommunityBikeSharing.ViewModels
{
	public class CommunityMembersViewModel : BaseViewModel
	{
		private readonly string _communityId;
		private CommunityMembership _currentUserMembership;
		private readonly IMembershipService _membershipService;
		private readonly IDialogService _dialogService;
		private readonly IUserService _userService;
		private readonly IAuthService _authService;

		private User _currentUser;

		private CommunityMembership CurrentUserMembership
		{
			get => _currentUserMembership;
			set
			{
				_currentUserMembership = value;
				OnPropertyChanged();
				OnPropertyChanged(nameof(AddMemberVisible));
			}
		}

		private ObservableCollection<CommunityMembership> _members;

		public ObservableCollection<CommunityMembership> Members
		{
			get => _members;
			set
			{
				if (_members != null)
				{
					_members.CollectionChanged -= _membersChanged;
				}

				_members = value;
				_members.CollectionChanged += _membersChanged;

				OnPropertyChanged();
				_membersChanged(null, null);
			}
		}

		private readonly NotifyCollectionChangedEventHandler _membersChanged;

		public bool AddMemberVisible => CurrentUserMembership is {Role: CommunityRole.CommunityAdmin};

		public IEnumerable<CommunityMembership> SortedMembers
			=> Members?.OrderBy(m => m.Role).ThenBy(m => m.Name);

		public ICommand EditMembershipCommand => new Command<CommunityMembership>(EditMembership);

		private void EditMembership(CommunityMembership membership)
		{
			var actions = new []
			{
				("Zum Community-Admin machen", PromoteCommunityAdminCommand),
				("Nutzer entfernen", RemoveMemberCommand)
			};

			_dialogService.ShowActionSheet(membership.Name, "Abbrechen", actions, membership);
		}

		public CommunityMembersViewModel(
			IMembershipService membershipService,
			IDialogService dialogService,
			IUserService userService,
			IAuthService authService,
			string communityId)
		{
			_membershipService = membershipService;
			_dialogService = dialogService;
			_userService = userService;
			_authService = authService;
			_communityId = communityId;
			_membersChanged = (sender, args) =>
			{
				OnPropertyChanged(nameof(SortedMembers));
				CurrentUserMembership = Members?.SingleOrDefault(m => m.UserId == _currentUser.Id);
			};
		}

		public override async Task InitializeAsync()
		{
			_authService.ObserveCurrentUser().Subscribe(user => _currentUser = user);
			Members = _membershipService.ObserveMembershipsFromCommunity(_communityId);
		}

		public ICommand AddMemberCommand => new Command(AddMember);

		private async void AddMember()
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
					SendInvitationMail(email);
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

		public ICommand PromoteCommunityAdminCommand => new Command<CommunityMembership>(
			PromoteCommunityAdmin, CanPromoteCommunityAdmin);
		public ICommand RemoveMemberCommand => new Command<CommunityMembership>(
			RemoveMember, CanRemoveMember);

		private async void PromoteCommunityAdmin(CommunityMembership membership)
		{
			await _membershipService.PromoteToCommunityAdmin(membership);
		}

		private bool CanPromoteCommunityAdmin(CommunityMembership membership) =>
			membership.Role == CommunityRole.User && UserIsAdminAndEditsOtherUser(membership);

		private async void RemoveMember(CommunityMembership membership)
		{
			await _membershipService.Delete(membership);
		}

		private bool CanRemoveMember(CommunityMembership membership) => UserIsAdminAndEditsOtherUser(membership);

		private bool UserIsAdminAndEditsOtherUser(CommunityMembership membership) =>
			CurrentUserMembership.Role == CommunityRole.CommunityAdmin && CurrentUserMembership.Id != membership.Id;

		private async void SendInvitationMail(string mailAddress)
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
