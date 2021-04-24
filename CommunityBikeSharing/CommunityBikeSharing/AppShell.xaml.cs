﻿using CommunityBikeSharing.Views;
using Xamarin.Forms;

namespace CommunityBikeSharing
{
	public partial class AppShell : Shell
	{
		public AppShell()
		{
			InitializeComponent();

			var routes = new[]
			{
				typeof(CommunityOverviewPage), typeof(CommunitySettingsPage)
			};

			foreach (var route in routes)
			{
				Routing.RegisterRoute(route.Name, route);
			}
		}
	}
}

