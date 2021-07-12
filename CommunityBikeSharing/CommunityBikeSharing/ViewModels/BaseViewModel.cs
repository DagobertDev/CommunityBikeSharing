using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.CommunityToolkit.ObjectModel;

namespace CommunityBikeSharing.ViewModels
{
	public class BaseViewModel : ObservableObject
	{
		public virtual Task InitializeAsync() => Task.CompletedTask;

		public ICommand CreateCommand(Action action) => CommandFactory.Create(action);
		public ICommand CreateCommand(Action action, Func<bool> canExecute) => CommandFactory.Create(action, canExecute);
		public ICommand CreateCommand<T>(Action<T> action) => CommandFactory.Create(action);
		public ICommand CreateCommand<T>(Action<T> action, Func<T, bool> canExecute) => CommandFactory.Create(action, canExecute);
		public ICommand CreateCommand(Func<Task> task) => CommandFactory.Create(task);
		public ICommand CreateCommand(Func<Task> task, Func<bool> canExecute) 
			=> CommandFactory.Create(task, canExecute);
		public ICommand CreateCommand<T>(Func<T, Task> task) => CommandFactory.Create(task!);
		public ICommand CreateCommand<T>(Func<T, Task> task, Func<T, bool> canExecute) 
			=> CommandFactory.Create(task!, o => o is T t && canExecute(t));
	}
}
