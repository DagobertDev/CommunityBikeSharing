using System.Threading.Tasks;
using CommunityBikeSharing.Services;
using Xamarin.Forms;

[assembly: Dependency(typeof(DialogService))]

namespace CommunityBikeSharing.Services
{
	public class DialogService : IDialogService
	{
		public async Task ShowMessage(string title, string message, string buttonText)
		{
			await Application.Current.MainPage.DisplayAlert(title, message, buttonText);
		}

		public Task ShowError(string title, string message, string buttonText) =>
			ShowMessage(title, message, buttonText);
	}
}
