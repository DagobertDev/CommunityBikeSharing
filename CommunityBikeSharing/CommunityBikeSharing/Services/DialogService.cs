using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CommunityBikeSharing.Services;
using Xamarin.Forms;

[assembly: Dependency(typeof(DialogService))]

namespace CommunityBikeSharing.Services
{
	public class DialogService : IDialogService
	{
		public Task ShowMessage(string title, string message, string buttonText)
			=> Application.Current.MainPage.DisplayAlert(title, message, buttonText);

		public Task ShowError(string title, string message, string buttonText)
			=> ShowMessage(title, message, buttonText);

		public Task<bool> ShowConfirmation(string title, string message, string confirm, string cancel)
			=> Application.Current.MainPage.DisplayAlert(title, message, confirm, cancel);

		public Task<string> ShowTextEditor(string title, string message, string confirm, string cancel,
			IDialogService.KeyboardType keyboardType)
		{
			var keyboard = keyboardType switch
			{
				IDialogService.KeyboardType.Default => Keyboard.Default,
				IDialogService.KeyboardType.Email => Keyboard.Email,
				_ => Keyboard.Default
			};

			return Application.Current.MainPage.DisplayPromptAsync(title, message, confirm, cancel, keyboard: keyboard);
		}

		public async Task ShowActionSheet(string title, string cancel, IEnumerable<(string, Action)> actionsEnumerable)
		{
			var actions = actionsEnumerable.ToArray();

			if (actions.Length == 0)
			{
				return;
			}

			var result = await Application.Current.MainPage.DisplayActionSheet(title, cancel, null,
				actions.Select(a => a.Item1).ToArray());

			actions.SingleOrDefault(action => action.Item1 == result).Item2?.Invoke();
		}
	}
}
