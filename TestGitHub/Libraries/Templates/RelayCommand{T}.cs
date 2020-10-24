using System;
using System.Windows.Input;

namespace TestGitHub.Libraries.Templates
{
    public class RelayCommand<T> : ICommand
    {
        private readonly Action<T> execute;

        private readonly Predicate<object> canExecute;

        public RelayCommand(Action<T> action)
            : this(action, null)
        {
        }

        public RelayCommand(Action<T> action, Predicate<object> canAction)
        {
            execute = action ?? throw new ArgumentNullException(nameof(action));
            canExecute = canAction;
        }

        public event EventHandler CanExecuteChanged
        {
            add
            {
                if (canExecute != null)
                {
                    CommandManager.RequerySuggested += value;
                }
            }

            remove
            {
                if (canExecute != null)
                {
                    CommandManager.RequerySuggested -= value;
                }
            }
        }

        public bool CanExecute(object parameter)
        {
            return canExecute == null || canExecute(parameter);
        }

        public void Execute(object parameter)
        {
            execute((T)parameter);
        }
    }
}
