using System;
using System.Threading.Tasks;

namespace CommunityBikeSharing.Services
{
	public interface IDialogService
	{
		Task ShowMessage(string title, string message, string buttonText = "Ok");
		Task ShowError(string title, string message, string buttonText = "Ok");
		Task<bool> ShowConfirmation(string title, string message, string confirm = "Ja", string cancel = "Abbrechen");
		Task<string> ShowTextEditor(string title, string message, string confirm = "Ok", string cancel = "Abbrechen",
			KeyboardType keyboard = KeyboardType.Default);
		Task ShowActionSheet(string title, string cancel, params (string, Action)[] actions);

		public enum KeyboardType
		{
			Default, Email
		}
	}
}
