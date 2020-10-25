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
            this.execute = action ?? throw new ArgumentNullException(nameof(action));
            this.canExecute = canAction;
        }

        public event EventHandler CanExecuteChanged
        {
            add
            {
                if (this.canExecute != null)
                {
                    CommandManager.RequerySuggested += value;
                }
            }

            remove
            {
                if (this.canExecute != null)
                {
                    CommandManager.RequerySuggested -= value;
                }
            }
        }

        public bool CanExecute(object parameter)
        {
            return this.canExecute == null || this.canExecute(parameter);
        }

        public void Execute(object parameter)
        {
            this.execute((T)parameter);
        }
    }
}
