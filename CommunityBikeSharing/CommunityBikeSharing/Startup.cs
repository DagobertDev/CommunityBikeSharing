using System;
using System.IO;
using System.Reflection;
using CommunityBikeSharing.Configuration;
using CommunityBikeSharing.Services;
using CommunityBikeSharing.Services.Data;
using CommunityBikeSharing.Services.Data.Bikes;
using CommunityBikeSharing.Services.Data.Communities;
using CommunityBikeSharing.Services.Data.Locks;
using CommunityBikeSharing.Services.Data.Memberships;
using CommunityBikeSharing.Services.Data.Stations;
using CommunityBikeSharing.Services.Data.Users;
using CommunityBikeSharing.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Xamarin.Forms;

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

			return ServiceProvider.GetService<App>()!;
		}

		private static void ConfigureServices(IServiceCollection services)
		{
			services.AddConfiguration()

				.AddSingleton<IAuthService, FirebaseAuthService>()
				.AddSingleton<IDialogService, DialogService>()
				.AddSingleton<ILocationPicker, LocationPicker>()
				.AddSingleton<ILocationService, LocationService>()
				.AddSingleton(MessagingCenter.Instance)
				.AddSingleton<INavigationService, NavigationService>()
				.AddSingleton<IQRCodeScanner, QRCodeScanner>()

				.AddSingleton<IFirestoreContext, FirestoreContext>()
				.AddRepositories()
				.AddDataServices()
				.AddViewModels()

				.AddSingleton<App>();
		}

		private static IServiceCollection AddConfiguration(this IServiceCollection services)
		{
			var stream = Assembly.GetAssembly(typeof(AppSettings))
				.GetManifestResourceStream("CommunityBikeSharing.Configuration.appsettings.json");

			if (stream == null)
			{
				throw new Exception("Could not find configuration");
			}

			using var reader = new StreamReader(stream);

			var json = reader.ReadToEnd();

			var appSettings = JsonConvert.DeserializeObject<AppSettings>(json);

			return services.AddSingleton(appSettings ?? throw new Exception("Could not load configuration"));
		}

		private static IServiceCollection AddRepositories(this IServiceCollection services) =>
			services.AddSingleton<IBikeRepository, BikeRepository>()
				.AddSingleton<ICommunityRepository, CommunityRepository>()
				.AddSingleton<ILockRepository, LockRepository>()
				.AddSingleton<IMembershipRepository, MembershipRepository>()
				.AddSingleton<IStationRepository, StationRepository>()
				.AddSingleton<IUserRepository, UserRepository>()
				.AddSingleton<IUserEmailRepository, UserEmailRepository>();

		private static IServiceCollection AddDataServices(this IServiceCollection services) =>
			services.AddSingleton<IBikeService, BikeService>()
				.AddSingleton<ICommunityService, CommunityService>()
				.AddSingleton<ILockControlService, LockItLockControlService>()
				.AddSingleton<ILockKeyService, LockKeyService>()
				.AddSingleton<ILockService, LockService>()
				.AddSingleton<IMembershipService, MembershipService>()
				.AddSingleton<IStationService, StationService>()
				.AddSingleton<IUserService, UserService>();

		private static IServiceCollection AddViewModels(this IServiceCollection services) =>
			services.AddTransient<BikeViewModel>()
				.AddTransient<CommunitiesViewModel>()
				.AddTransient<CommunityBikesViewModel>()
				.AddTransient<CommunityMembersViewModel>()
				.AddTransient<CommunityOverviewViewModel>()
				.AddTransient<CommunityStationsViewModel>()
				.AddTransient<EditBikeViewModel>()
				.AddTransient<EditStationViewModel>()
				.AddTransient<LicenseDetailViewModel>()
				.AddTransient<LicensesViewModel>()
				.AddTransient<LoadingViewModel>()
				.AddTransient<StartupViewModel>()
				.AddTransient<LoginViewModel>()
				.AddTransient<MapModalViewModel>()
				.AddTransient<OverviewViewModel>()
				.AddTransient<ProfileViewModel>()
				.AddTransient<RegistrationViewModel>()
				.AddTransient<StationDetailViewModel>();
	}
}
