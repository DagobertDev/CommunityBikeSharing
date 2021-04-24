using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CommunityBikeSharing.Models;
using CommunityBikeSharing.Services;
using Xamarin.Forms;

namespace CommunityBikeSharing.ViewModels
{
	public class CommunityMembersViewModel : BaseViewModel
	{
		private readonly string _communityId;
		private readonly ICommunityRepository _communityRepository;
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
		}

		public async Task InitializeAsync()
		{
			Members = await _communityRepository.GetCommunityMembers(_communityId);
		}
	}
}
