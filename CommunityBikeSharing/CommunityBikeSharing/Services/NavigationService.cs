using System.Threading.Tasks;
using CommunityBikeSharing.ViewModels;
using Xamarin.Forms;

namespace CommunityBikeSharing.Services
{
	public class NavigationService : INavigationService
	{
		public Task NavigateTo<T>() where T : BaseViewModel => Shell.Current.GoToAsync(typeof(T).Name);
		public Task NavigateToRoot<T>() where T : BaseViewModel => Shell.Current.GoToAsync($"///{typeof(T).Name}");
		public Task NavigateBack() => Shell.Current.GoToAsync("..");
		public Task NavigateTo<T>(string parameter) where T : BaseViewModel =>
			Shell.Current.GoToAsync($"{typeof(T).Name}?param={parameter}");
	}
}
