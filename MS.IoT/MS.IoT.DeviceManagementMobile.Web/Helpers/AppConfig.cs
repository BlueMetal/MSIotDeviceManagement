using System.Configuration;

namespace MS.IoT.DeviceManagementMobile.Web.Helpers
{
    public static class AppConfig
    {
        public static class ConfigurationItems
        {
            public static string AppInsightsInstrumentationKey
            {
                get
                {
                    return ConfigurationManager.AppSettings["APPINSIGHTS_INSTRUMENTATIONKEY"];
                }
            }

            public static string IoTHubConnectionString
            {
                get
                {
                    return ConfigurationManager.AppSettings["iotHub:ConnectionString"];
                }
            }

            public static string NotificationHubConnectionString
            {
                get
                {
                    return ConfigurationManager.AppSettings["notificationHub:ConnectionString"];
                }
            }
        }
    }
}