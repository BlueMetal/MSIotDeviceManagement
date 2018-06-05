using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MS.IoT.Simulator.WPF.Helpers
{
    /// <summary>
    /// https://msdn.microsoft.com/en-us/magazine/dn630647.aspx
    /// </summary>
    /// <typeparam name="TResult"></typeparam>
    public abstract class AsyncCommandBase : IAsyncCommand
    {
        private object _Parameter;

        public object Parameter
        {
            get
            {
                return _Parameter;
            }
        }

        public abstract bool CanExecute(object parameter);
        public abstract Task ExecuteAsync(object parameter);
        public async void Execute(object parameter)
        {
            _Parameter = parameter;
            await ExecuteAsync(parameter);
        }
        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }
        protected void RaiseCanExecuteChanged()
        {
            CommandManager.InvalidateRequerySuggested();
        }
    }
}
