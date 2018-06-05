using System.Configuration;

namespace MS.IoT.Simulator.WPF.Helpers
{
    /// <summary>
    /// AppConfig
    /// Class used to retrieve the AppConfig keys
    /// </summary>
    public static class AppConfig
    {
        public static class CosmosDB
        {
            public static string EndPoint
            {
                get
                {
                    return ConfigurationManager.AppSettings["cdb:EndPoint"];
                }
            }

            public static string AuthKey
            {
                get
                {
                    return ConfigurationManager.AppSettings["cdb:AuthKey"];
                }
            }

            public static string Database
            {
                get
                {
                    return ConfigurationManager.AppSettings["cdb:Database"];
                }
            }

            public static string CollectionTemplate
            {
                get
                {
                    return ConfigurationManager.AppSettings["cdb:ColTemplates"];
                }
            }

            public static string CollectionMessage
            {
                get
                {
                    return ConfigurationManager.AppSettings["cdb:ColMessages"];
                }
            }
        }

        public static class IoTHub
        {
            public static string HostName
            {
                get
                {
                    return ConfigurationManager.AppSettings["iotHub:HostName"];
                }
            }

            public static string ConnectionString
            {
                get
                {
                    return ConfigurationManager.AppSettings["iotHub:ConnectionString"];
                }
            }

            public static string SimulatorDeviceName
            {
                get
                {
                    return ConfigurationManager.AppSettings["iotHub:SimulatorDeviceName"];
                }
            }
        }
    }
}