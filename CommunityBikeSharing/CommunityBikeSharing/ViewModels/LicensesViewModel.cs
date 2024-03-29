﻿using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using CommunityBikeSharing.Licenses;
using CommunityBikeSharing.Services;

namespace CommunityBikeSharing.ViewModels
{
	public class LicensesViewModel : BaseViewModel
	{
		private readonly INavigationService _navigationService;

		public LicensesViewModel(INavigationService navigationService)
		{
			_navigationService = navigationService;

			var assembly = Assembly.GetAssembly(typeof(LicensesViewModel));

			Licenses = assembly.GetManifestResourceNames()
				.Where(file => file.EndsWith("LICENSE"))
				.Select(file => new License(file))
				.ToList();
		}

		public List<License> Licenses { get; }

		public Task OnLicenseSelected(License license)
			=> _navigationService.NavigateTo<LicenseDetailViewModel>(license.Path);
	}
}
