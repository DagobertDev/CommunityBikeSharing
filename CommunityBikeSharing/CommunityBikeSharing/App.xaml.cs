using System;
using CommunityBikeSharing.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using Xamarin.Forms;

namespace CommunityBikeSharing
{
    public partial class App
    {
	    public App()
        {
            InitializeComponent();

            MainPage = new AppShell();
        }

	    public static T GetViewModel<T>() => Startup.ServiceProvider.GetService<T>()
	                                         ?? throw new ArgumentException($"{typeof(T).Name} is not a registered view model");
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
