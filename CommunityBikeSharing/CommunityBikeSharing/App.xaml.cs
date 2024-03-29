﻿using Microsoft.Extensions.DependencyInjection;

namespace CommunityBikeSharing
{
    public partial class App
    {
	    public App()
        {
            InitializeComponent();

            MainPage = new AppShell();
        }

	    public static T GetViewModel<T>() where T : notnull => Startup.ServiceProvider.GetRequiredService<T>();

	    public static T GetViewModel<T>(params object[] parameter) =>
		    ActivatorUtilities.CreateInstance<T>(Startup.ServiceProvider, parameter);

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
