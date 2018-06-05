using MS.IoT.UWP.CoffeeMaker.Models.Packets;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace MS.IoT.UWP.CoffeeMaker.Helpers
{
    /// <summary>
    /// TimeoutAsyncEvent
    /// Helper that adds a timeout to an async task with FourByFourReception object.
    /// </summary>
    public static class TimeoutAsyncEvent
    {
        public static async Task<DataPacket> TimeoutAfter(this Task<DataPacket> task, TimeSpan timeOut)
        {
            var tokenSource = new CancellationTokenSource();
            Task timeoutTask = Task.Delay(timeOut, tokenSource.Token);
            if (task == await Task.WhenAny(task, timeoutTask))
            {
                tokenSource.Cancel();
                tokenSource.Dispose();
                return await task;
            }
            else
            {
                return null;
            }
        }
    }
}
