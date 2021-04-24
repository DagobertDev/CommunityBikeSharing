﻿using System.Threading.Tasks;
using CommunityBikeSharing.Services;
using Xamarin.Forms;

namespace CommunityBikeSharing.ViewModels
{
	public class CommunityOverviewViewModel : BaseViewModel
	{
		private readonly string _id;
		private readonly ICommunityRepository _communityRepository;
		private string _name;

		public string Name
		{
			get => _name;
			set
			{
				_name = value;
				OnPropertyChanged();
			}
		}

		public CommunityOverviewViewModel(string id)
		{
			_id = id;
			_communityRepository = DependencyService.Get<ICommunityRepository>();
		}

		public async Task InitializeAsync()
		{
			var community = await _communityRepository.GetCommunity(_id);
			Name = community.Name;
		}
	}
}
