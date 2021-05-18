using System;
using CommunityBikeSharing.Services;
using CommunityBikeSharing.Services.Data;
using CommunityBikeSharing.Services.Data.Bikes;
using CommunityBikeSharing.Services.Data.Communities;
using CommunityBikeSharing.Services.Data.Memberships;
using CommunityBikeSharing.Services.Data.Stations;
using CommunityBikeSharing.Services.Data.Users;
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
			services.AddSingleton<IAuthService, FirebaseAuthService>()
				.AddSingleton<IDialogService, DialogService>()
				.AddSingleton<ILocationPicker, LocationPicker>()
				.AddSingleton<ILocationService, LocationService>()
				.AddSingleton<INavigationService, NavigationService>()

				.AddSingleton<IFirestoreContext, FirestoreContext>()
				.AddRepositories()
				.AddDataServices()
				.AddViewModels()

				.AddSingleton<App>();
		}

		private static IServiceCollection AddRepositories(this IServiceCollection services) =>
			services.AddSingleton<IBikeRepository, BikeRepository>()
				.AddSingleton<ICommunityRepository, CommunityRepository>()
				.AddSingleton<IMembershipRepository, MembershipRepository>()
				.AddSingleton<IStationRepository, StationRepository>()
				.AddSingleton<IUserRepository, UserRepository>()
				.AddSingleton<IUserEmailRepository, UserEmailRepository>();

		private static IServiceCollection AddDataServices(this IServiceCollection services) =>
			services.AddSingleton<IBikeService, BikeService>()
				.AddSingleton<ICommunityService, CommunityService>()
				.AddSingleton<IMembershipService, MembershipService>()
				.AddSingleton<IStationService, StationService>()
				.AddSingleton<IUserService, UserService>();

		private static IServiceCollection AddViewModels(this IServiceCollection services) =>
			services.AddTransient<CommunitiesViewModel>()
				.AddTransient<CommunityBikesViewModel>()
				.AddTransient<CommunityMembersViewModel>()
				.AddTransient<CommunityOverviewPage>()
				.AddTransient<CommunityStationsPage>()
				.AddTransient<EditStationViewModel>()
				.AddTransient<LoadingViewModel>()
				.AddTransient<LoginViewModel>()
				.AddTransient<MapModalViewModel>()
				.AddTransient<OverviewViewModel>()
				.AddTransient<ProfileViewModel>()
				.AddTransient<RegistrationViewModel>()
				.AddTransient<StationDetailViewModel>();
	}
}
