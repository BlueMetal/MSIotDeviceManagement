using MS.IoT.UWP.CoffeeMaker.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MS.IoT.UWP.CoffeeMaker.Helpers
{
    public class LogHelper
    {
        public delegate void LogEntryHandler(Object sender, string e);
        public static event LogEntryHandler LogEntry;

        public static void LogInfo(string message, DataSource dataSource = DataSource.Other)
        {
            string formatedMessage = string.Format("{0,-6}{1,-6}{2}\n", "INFO", DateTime.Now.ToLocalTime().ToString("HH:mm"), message);

            LogEntry?.Invoke(null, formatedMessage);
        }

        public static void LogError(string message, DataSource dataSource = DataSource.Other)
        {
            string formatedMessage = string.Format("{0,-6}{1,-6}{2}\n", "ERROR", DateTime.Now.ToLocalTime().ToString("HH:mm"), message);

            LogEntry?.Invoke(null, formatedMessage);
        }
    }
}
