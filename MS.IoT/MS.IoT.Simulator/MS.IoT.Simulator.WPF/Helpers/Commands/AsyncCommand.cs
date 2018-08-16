using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace MS.IoT.Simulator.WPF.Helpers
{
    /// <summary>
    /// https://msdn.microsoft.com/en-us/magazine/dn630647.aspx
    /// </summary>
    /// <typeparam name="TResult"></typeparam>
    public class AsyncCommand<TResult> : AsyncCommandBase, INotifyPropertyChanged
    {
        private readonly Func<Task<TResult>> _Command;
        private NotifyTaskCompletion<TResult> _Execution;

        public AsyncCommand(Func<Task<TResult>> command)
        {
            _Command = command;
        }

        public override bool CanExecute(object parameter)
        {
            return true;
        }

        public override Task ExecuteAsync(object parameter)
        {
            Execution = new NotifyTaskCompletion<TResult>(_Command());
            return Execution.TaskCompletion;
        }

        public NotifyTaskCompletion<TResult> Execution
        {
            get { return _Execution; }
            private set
            {
                _Execution = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public static class AsyncCommand
    {
        public static AsyncCommand<object> Create(Func<Task> command)
        {
            return new AsyncCommand<object>(async () => { await command(); return null; });
        }

        public static AsyncCommand<TResult> Create<TResult>(Func<Task<TResult>> command)
        {
            return new AsyncCommand<TResult>(command);
        }
    }
}
