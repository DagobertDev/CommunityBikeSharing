using System;
using System.Threading.Tasks;
using CommunityBikeSharing.ViewModels;
using Xamarin.Forms;

namespace CommunityBikeSharing.Services
{
	public class NavigationService : INavigationService
	{
		public Task NavigateTo<T>() where T : BaseViewModel => Shell.Current.GoToAsync(typeof(T).Name);
		public Task NavigateToRoot<T>() where T : BaseViewModel => Shell.Current.GoToAsync($"///{typeof(T).Name}");
		public Task NavigateTo<T>(params string[] parameter) where T : BaseViewModel
		{
			return parameter.Length switch
			{
				0 => Shell.Current.GoToAsync($"{typeof(T).Name}"),
				1 => Shell.Current.GoToAsync($"{typeof(T).Name}?param={parameter[0]}"),
				2 => Shell.Current.GoToAsync($"{typeof(T).Name}?param={parameter[0]}&param2={parameter[1]}"),
				_ => throw new NotImplementedException()
			};
		}

		public Task NavigateBack() => Shell.Current.GoToAsync("..");
	}
}
