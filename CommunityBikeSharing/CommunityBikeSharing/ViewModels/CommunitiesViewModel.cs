using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using CommunityBikeSharing.Models;
using CommunityBikeSharing.Services;
using Xamarin.Forms;

namespace CommunityBikeSharing.ViewModels
{
	public class CommunitiesViewModel : BaseViewModel
	{
		private ObservableCollection<Community> _communities;

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

		public CommunitiesViewModel()
		{
			_communityRepository = DependencyService.Get<ICommunityRepository>();
			AddCommunityCommand = new Command(AddCommunity);
		}

		public ICommand AddCommunityCommand { get; }

		private void AddCommunity()
		{
			_communityRepository.AddCommunity(new Community
			{
				Name = "Test"
			});
		}

		public async Task InitializeAsync()
		{
			Communities = _communityRepository.ObserveCommunities();
		}
	}
}
