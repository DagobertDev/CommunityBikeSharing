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
		private readonly IDialogService _dialogService;

		public CommunitiesViewModel()
		{
			_communityRepository = DependencyService.Get<ICommunityRepository>();
			_dialogService = DependencyService.Get<IDialogService>();
			AddCommunityCommand = new Command(AddCommunity);
		}

		public ICommand AddCommunityCommand { get; }

		private async void AddCommunity()
		{
			var name = await _dialogService.ShowTextEditor("Name der Community", "Ok", "Abbrechen");

			if (string.IsNullOrEmpty(name))
			{
				return;
			}

			await _communityRepository.AddCommunity(new Community
			{
				Name = name
			});
		}

		public async Task InitializeAsync()
		{
			Communities = _communityRepository.ObserveCommunities();
		}
	}
}
