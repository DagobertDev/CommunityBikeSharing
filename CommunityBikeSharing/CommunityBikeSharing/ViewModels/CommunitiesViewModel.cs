using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using CommunityBikeSharing.Models;
using CommunityBikeSharing.Services;
using CommunityBikeSharing.Services.Data;
using Xamarin.Forms;

namespace CommunityBikeSharing.ViewModels
{
	public class CommunitiesViewModel : BaseViewModel
	{
		private ObservableCollection<Community> _communities;
		private User _user;

		public ObservableCollection<Community> Communities
		{
			get => _communities;
			set
			{
				_communities = value;
				OnPropertyChanged();
			}
		}

		private readonly ICommunityRepository _communityRepository;
		private readonly IDialogService _dialogService;
		private readonly INavigationService _navigationService;
		private readonly IMembershipRepository _membershipRepository;
		private readonly IAuthService _authService;

		public CommunitiesViewModel(
			ICommunityRepository communityRepository,
			IDialogService dialogService,
			INavigationService navigationService,
			IMembershipRepository membershipRepository,
			IAuthService authService)
		{
			_communityRepository = communityRepository;
			_dialogService = dialogService;
			_navigationService = navigationService;
			_membershipRepository = membershipRepository;
			_authService = authService;
		}

		public ICommand AddCommunityCommand => new Command(AddCommunity);

		private async void AddCommunity()
		{
			var name = await _dialogService.ShowTextEditor("Name der Community",
				"Bitte geben Sie den Namen der neuen Community ein");

			if (string.IsNullOrEmpty(name))
			{
				return;
			}

			var community = await _communityRepository.Create(name, _user);
			OpenCommunityDetail(community);
		}

		public ICommand OpenCommunityDetailCommand => new Command<Community>(OpenCommunityDetail);

		private async void OpenCommunityDetail(Community community)
		{
			await _navigationService.NavigateTo<CommunityOverviewViewModel>(community.Id);
		}

		public override async Task InitializeAsync()
		{
			_user = _authService.GetCurrentUser();
			var memberships = _membershipRepository.ObserveMembershipsFromUser(_user.Id);
			Communities = await _communityRepository.GetCommunities(memberships);
		}
	}
}
