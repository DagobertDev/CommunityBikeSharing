#nullable enable
using System.Threading.Tasks;
using CommunityBikeSharing.ViewModels;

namespace CommunityBikeSharing.Services
{
	public interface INavigationService
	{
		public Task NavigateTo<T>() where T : BaseViewModel;
		public Task NavigateToRoot<T>() where T : BaseViewModel;
		public Task NavigateTo<T>(params string[] parameter) where T : BaseViewModel;
		public Task NavigateBack();
	}
}
