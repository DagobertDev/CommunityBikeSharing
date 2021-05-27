using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace CommunityBikeSharing.Services
{
	public interface IDialogService
	{
		Task ShowMessage(string title, string message, string buttonText = "Ok");
		Task ShowError(string title, string message, string buttonText = "Ok");
		Task<bool> ShowConfirmation(string title, string message, string confirm = "Ja", string cancel = "Abbrechen");
		Task<string> ShowTextEditor(string title, string message, string confirm = "Ok", string cancel = "Abbrechen",
			KeyboardType keyboard = KeyboardType.Default);

		Task ShowActionSheet(string title, string cancel, IEnumerable<(string, Action)> actions);

		Task ShowActionSheet(string title, string cancel, IEnumerable<(string, ICommand)> commands, object param = null)
		{
			var actions = commands
				.Where(c => c.Item2.CanExecute(param))
				.Select(c => (c.Item1, new Action(() => c.Item2.Execute(param))));

			return ShowActionSheet(title, cancel, actions);
		}

		public enum KeyboardType
		{
			Default, Email, Numeric
		}
	}
}
