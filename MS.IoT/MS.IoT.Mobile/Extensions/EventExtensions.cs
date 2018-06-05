using System;
using System.Collections.Generic;
using System.Text;

namespace System
{
    public static class EventExtensions
    {


        public static void SafeTrigger<TEventArgs>
                (this EventHandler<TEventArgs> eventToTrigger,
                Object sender, TEventArgs eventArgs)
                where TEventArgs : EventArgs
        {
            eventToTrigger?.Invoke(sender, eventArgs);
        }




    }
}
