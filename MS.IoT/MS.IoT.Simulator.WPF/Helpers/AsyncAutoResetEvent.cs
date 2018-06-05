using MS.IoT.Simulator.WPF.Models.Packets;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace MS.IoT.Simulator.WPF.Helpers
{
    /// <summary>
    /// TimeoutAsyncEvent
    /// Helper that adds a timeout to an async task with FourByFourReception object.
    /// </summary>
    public static class TimeoutAsyncEvent
    {
        public static async Task<FourByFourReception> TimeoutAfter(this Task<FourByFourReception> task, TimeSpan timeOut)
        {
            var tokenSource = new CancellationTokenSource();
            Task timeoutTask = Task.Delay(timeOut, tokenSource.Token);
            if (task == await Task.WhenAny(task, timeoutTask))
            {
                tokenSource.Cancel();
                tokenSource.Dispose();
                timeoutTask.Dispose();
                return await task;
            }
            else
            {
                return new StatusReception("TIME", ReceptionState.Timeout);
            }
        }
    }
}
