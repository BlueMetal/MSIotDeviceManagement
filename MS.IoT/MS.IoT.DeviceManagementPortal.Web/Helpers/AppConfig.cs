using Microsoft.Azure.ActiveDirectory.GraphClient;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace MS.IoT.DeviceManagementPortal.Web.Helpers
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

            public static string CosmosDBEndPoint
            {
                get
                {
                    return ConfigurationManager.AppSettings["cdb:EndPoint"];
                }
            }

            public static string CosmosDBAuthKey
            {
                get
                {
                    return ConfigurationManager.AppSettings["cdb:AuthKey"];
                }
            }

            public static string CosmosDBName
            {
                get
                {
                    return ConfigurationManager.AppSettings["cdb:Database"];
                }
            }

            public static string CosmosDBDevicesCollectionName
            {
                get
                {
                    return ConfigurationManager.AppSettings["cdb:ColDevices"];
                }
            }

            public static string CosmosDBGroupsCollectionName
            {
                get
                {
                    return ConfigurationManager.AppSettings["cdb:ColGroups"];
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

            public static string BingMapsKey
            {
                get
                {
                    return ConfigurationManager.AppSettings["bingMapsApiKey"];
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