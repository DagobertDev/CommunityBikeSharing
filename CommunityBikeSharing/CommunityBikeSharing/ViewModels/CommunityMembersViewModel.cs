using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using CommunityBikeSharing.Models;
using CommunityBikeSharing.Services;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace CommunityBikeSharing.ViewModels
{
	public class CommunityMembersViewModel : BaseViewModel
	{
		private readonly string _communityId;
		private readonly IMembershipRepository _membershipRepository;
		private readonly IDialogService _dialogService;
		private readonly IUserRepository _userRepository;

		private ObservableCollection<CommunityMembership> _members;

		public ObservableCollection<CommunityMembership> Members
		{
			get => _members;
			set
			{
				_members = value;
				OnPropertyChanged();
			}
		}

		public CommunityMembersViewModel(string communityId)
		{
			_communityId = communityId;
			_membershipRepository = DependencyService.Get<IMembershipRepository>();
			_dialogService = DependencyService.Get<IDialogService>();
			_userRepository = DependencyService.Get<IUserRepository>();

			AddMemberCommand = new Command(AddMember);
		}

		public async Task InitializeAsync()
		{
			Members = _membershipRepository.ObserveMembershipsFromCommunity(_communityId);
		}

		public ICommand AddMemberCommand { get; }

		private async void AddMember()
		{
			var email = await _dialogService.ShowTextEditor("Nutzer hinzufügen",
				"Geben Sie die Email-Adresse des neuen Mitgliedes ein:",
				"Hinzufügen");

			if (string.IsNullOrEmpty(email))
			{
				return;
			}

			var user = await _userRepository.GetUserByEmail(email);

			if (user == null)
			{
				var sendMail = await _dialogService.ShowConfirmation("Nutzer nicht gefunden",
					$"Es konnte kein Nutzer mit der Email \"{email}\" gefunden werden. " +
					$"Möchten Sie eine Einladung an \"{email}\" versenden?", "Mail-App öffnen");

				if (sendMail)
				{
					SendInvitationMail(email);
				}
			}
			else
			{
				await _membershipRepository.Add(new CommunityMembership
				{
					Name = user.Username,
					CommunityId = _communityId,
					UserId = user.Id
				});
			}
		}

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
