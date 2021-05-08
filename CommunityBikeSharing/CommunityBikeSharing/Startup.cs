using System;
using CommunityBikeSharing.Services;
using CommunityBikeSharing.Services.Data;
using CommunityBikeSharing.ViewModels;
using CommunityBikeSharing.Views;
using Microsoft.Extensions.DependencyInjection;

namespace CommunityBikeSharing
{
	public static class Startup
	{
		public static IServiceProvider ServiceProvider { get; private set; }

		public static App Init(Action<IServiceCollection> nativeConfigureServices)
		{
			var services = new ServiceCollection();

			nativeConfigureServices(services);
			ConfigureServices(services);

			ServiceProvider = services.BuildServiceProvider();

			return ServiceProvider.GetService<App>();
		}

		private static void ConfigureServices(IServiceCollection services)
		{
			services.AddSingleton<IAuthService, FirebaseAuthService>();
			services.AddSingleton<IBikeRepository, BikeRepository>();
			services.AddSingleton<IBikeService, BikeService>();
			services.AddSingleton<ICommunityRepository, CommunityRepository>();
			services.AddSingleton<IDialogService, DialogService>();
			services.AddSingleton<IFirestoreContext, FirestoreContext>();
			services.AddSingleton<IMembershipRepository, MembershipRepository>();
			services.AddSingleton<INavigationService, NavigationService>();
			services.AddSingleton<IUserRepository, UserRepository>();

			services.AddTransient<CommunitiesViewModel>();
			services.AddTransient<CommunityBikesViewModel>();
			services.AddTransient<CommunityMembersViewModel>();
			services.AddTransient<CommunityOverviewPage>();
			services.AddTransient<LoadingViewModel>();
			services.AddTransient<LoginViewModel>();
			services.AddTransient<OverviewViewModel>();
			services.AddTransient<ProfileViewModel>();
			services.AddTransient<RegistrationViewModel>();

			services.AddSingleton<App>();
		}
	}
}
