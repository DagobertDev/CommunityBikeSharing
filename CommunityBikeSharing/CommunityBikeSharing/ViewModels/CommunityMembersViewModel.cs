using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using CommunityBikeSharing.Models;
using CommunityBikeSharing.Services;
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
			var email = await _dialogService.ShowTextEditor("Nutzer hinzufügen", "Ok", "Abbrechen");

			if (string.IsNullOrEmpty(email))
			{
				return;
			}

			var user = await _userRepository.GetUserByEmail(email);

			if (user == null)
			{
				await _dialogService.ShowError("Nutzer nicht gefunden",
					$"Der Nutzer mit der Email \"{email}\" konnte nicht gefunden werden", "Ok");
			}

			await _communityRepository.AddUserToCommunity(user, _communityId);
		}
	}
}
