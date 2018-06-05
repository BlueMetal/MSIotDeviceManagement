using System.Configuration;

namespace MS.IoT.DataPacketDesigner.Web
{
    /// <summary>
    /// Class helper to retrieve the properties from the appconfig
    /// </summary>
    public static class AppConfig
    {
        public static class ConfigurationItems
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

            public static string IoTHubHostName
            {
                get
                {
                    return ConfigurationManager.AppSettings["iotHub:HostName"];
                }
            }

            public static string IoTHubConnectionString
            {
                get
                {
                    return ConfigurationManager.AppSettings["iotHub:ConnectionString"];
                }
            }

            public static string BlobConnectionString
            {
                get
                {
                    return ConfigurationManager.AppSettings["blob:ConnectionString"];
                }
            }

            public static string SimulateToolsContainerName
            {
                get
                {
                    return ConfigurationManager.AppSettings["blob:SimulateToolsContainerName"];
                }
            }           

            public static string AppInsightsInstrumentationKey
            {
                get
                {
                    return ConfigurationManager.AppSettings["APPINSIGHTS_INSTRUMENTATIONKEY"];
                }
            }
        }
    }
}