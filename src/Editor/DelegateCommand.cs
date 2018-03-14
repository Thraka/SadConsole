using System;
using System.Windows.Input;

namespace Editor
{
    public class DelegateCommand: ICommand
    {
        public DelegateCommand(Action<object> execute)
        {
            if (execute == null)
            {
                throw new ArgumentNullException("execute");
            }

            _execute = execute;
        }

        public DelegateCommand(Func<object, bool> canExecute, Action<object> execute)
        {
            if (canExecute == null)
            {
                throw new ArgumentNullException("canExecute");
            }
            if (execute == null)
            {
                throw new ArgumentNullException("execute");
            }

            _canExecute = canExecute;
            _execute = execute;
        }
        
        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            return _canExecute == null || _canExecute(parameter);
        }

        public void Execute(object parameter)
        {
            _execute(parameter);
        }

        public void RaiseCanExecuteChanged()
        {
            var handler = CanExecuteChanged;
            if (handler != null)
            {
                handler(this, System.EventArgs.Empty);
            }
        }

        #region Private members
        private Func<object, bool> _canExecute;
        private Action<object> _execute;
        #endregion
    }
}