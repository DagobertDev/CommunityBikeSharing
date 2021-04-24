using System.Threading.Tasks;

namespace CommunityBikeSharing.Services
{
	public interface IDialogService
	{
		Task ShowMessage(string title, string message, string buttonText);
		Task ShowError(string title, string message, string buttonText);
		Task<string> ShowTextEditor(string title, string confirm, string cancel);
	}
}
