using System;
using System.Windows.Input;

namespace Insyston.Operations.WPF.ViewModels.Collections.Controls
{
    public class DelegateCommand<T> : ICommand
    {

        private readonly Predicate<T> canExecute;
        private readonly Action<T> execute;

        public event EventHandler CanExecuteChanged;

        public DelegateCommand(Action<T> execute)
            : this(execute, null)
        {
        }

        public DelegateCommand(Action<T> execute,
            Predicate<T> canExecute)
        {
            this.execute = execute;
            this.canExecute = canExecute;
        }

        public bool CanExecute(object parameter)
        {
            if (this.canExecute == null)
            {
                return true;
            }

            return this.canExecute((T) parameter);
        }

        public void Execute(object parameter)
        {
            execute((T) parameter);
        }

        public void RaiseCanExecuteChanged()
        {
            if (CanExecuteChanged != null)
            {
                CanExecuteChanged(this, EventArgs.Empty);
            }
        }
    }
}
