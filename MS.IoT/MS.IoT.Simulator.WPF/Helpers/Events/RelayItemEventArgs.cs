using System;

namespace MS.IoT.Simulator.WPF.Helpers
{
    /// <summary>
    /// RelayItemEventArgs<T>
    /// Class used while firing custom events
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class RelayItemEventArgs<T> : EventArgs
    {
        /// <summary>
        /// Custom object to pass
        /// </summary>
        public T Object { get; private set; }

        public RelayItemEventArgs(T obj)
        {
            Object = obj;
        }
    }
}
