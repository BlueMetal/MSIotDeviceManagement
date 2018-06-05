using System.Threading.Tasks;
using System.Windows.Input;

namespace MS.IoT.Simulator.WPF.Helpers
{
    /// <summary>
    /// https://msdn.microsoft.com/en-us/magazine/dn630647.aspx
    /// </summary>
    /// <typeparam name="TResult"></typeparam>
    public interface IAsyncCommand : ICommand
    {
        Task ExecuteAsync(object parameter);
    }
}
