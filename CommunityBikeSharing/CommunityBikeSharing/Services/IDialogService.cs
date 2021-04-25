using System.Threading.Tasks;

namespace CommunityBikeSharing.Services
{
	public interface IDialogService
	{
		Task ShowMessage(string title, string message, string buttonText = "Ok");
		Task ShowError(string title, string message, string buttonText = "Ok");
		Task<bool> ShowConfirmation(string title, string message, string confirm = "Ok", string cancel = "Abbrechen");
		Task<string> ShowTextEditor(string title, string message, string confirm = "Ok", string cancel = "Abbrechen");
	}
}
