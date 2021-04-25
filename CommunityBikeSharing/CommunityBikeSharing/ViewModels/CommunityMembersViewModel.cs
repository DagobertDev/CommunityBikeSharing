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
		private readonly ICommunityRepository _communityRepository;
		private readonly IDialogService _dialogService;
		private readonly IUserRepository _userRepository;

		private ObservableCollection<CommunityMember> _members;

		public ObservableCollection<CommunityMember> Members
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
			_communityRepository = DependencyService.Get<ICommunityRepository>();
			_dialogService = DependencyService.Get<IDialogService>();
			_userRepository = DependencyService.Get<IUserRepository>();

			AddMemberCommand = new Command(AddMember);
		}

		public async Task InitializeAsync()
		{
			Members = await _communityRepository.GetCommunityMembers(_communityId);
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
				await _communityRepository.AddUserToCommunity(user, _communityId);
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
