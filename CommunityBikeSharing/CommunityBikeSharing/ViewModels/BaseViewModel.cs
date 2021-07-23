using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.CommunityToolkit.ObjectModel;

namespace CommunityBikeSharing.ViewModels
{
	public class BaseViewModel : ObservableObject
	{
		public virtual Task InitializeAsync() => Task.CompletedTask;

		protected ICommand CreateCommand(Action action) => CommandFactory.Create(action);
		protected ICommand CreateCommand(Action action, Func<bool> canExecute) => CommandFactory.Create(action, canExecute);
		protected ICommand CreateCommand<T>(Action<T> action) => CommandFactory.Create(action);
		protected ICommand CreateCommand<T>(Action<T> action, Func<T, bool> canExecute) => CommandFactory.Create(action, canExecute);
		protected ICommand CreateCommand(Func<Task> task) => CommandFactory.Create(task);
		protected ICommand CreateCommand(Func<Task> task, Func<bool> canExecute) 
			=> CommandFactory.Create(task, canExecute);
		protected ICommand CreateCommand<T>(Func<T, Task> task) => CommandFactory.Create(task!);
		protected ICommand CreateCommand<T>(Func<T, Task> task, Func<T, bool> canExecute) 
			=> CommandFactory.Create(task!, o => o is T t && canExecute(t));
	}
}
