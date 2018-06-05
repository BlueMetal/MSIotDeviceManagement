using Microsoft.Azure.ActiveDirectory.GraphClient;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace MS.IoT.MarketingPortal.Web.Helpers
{
    public static class AppConfig
    {
        public static class ConfigurationItems
        {        
            public static string ArmTemplateUrl
            {
                get
                {
                    return ConfigurationManager.AppSettings["armTemplateUrl"];
                }
            }

            public static string ArmTemplateStreamAnalyticsUrl
            {
                get
                {
                    return ConfigurationManager.AppSettings["armTemplateStreamAnalyticsUrl"];
                }
            }

            public static string DataPacketDesignerPackageWebZipuri
            {
                get
                {
                    return ConfigurationManager.AppSettings["dataPacketDesignerPackageWebZipUri"] + SasToken;
                }
            }

            public static string DeviceManagementPortalPackageWebZipUri
            {
                get
                {
                    return ConfigurationManager.AppSettings["deviceManagementPortalPackageWebZipUri"] + SasToken;
                }
            }

            public static string WatchVideoUrl
            {
                get
                {
                    return ConfigurationManager.AppSettings["watchVideoUrl"] + SasToken;
                }
            }

            public static string SasToken
            {
                get
                {
                    return ConfigurationManager.AppSettings["sasToken"];
                }
            }

            public static string CosmosDbTemplatesBaseUrl
            {
                get
                {
                    return ConfigurationManager.AppSettings["cosmosDbTemplatesBaseUrl"];
                }
            }

            public static string CosmosDbDatabaseName
            {
                get
                {
                    return ConfigurationManager.AppSettings["cdb:Database"];
                }
            }

            public static string CosmosDbCollectionNameTemplates
            {
                get
                {
                    return ConfigurationManager.AppSettings["cdb:ColTemplates"];
                }
            }

            public static string CosmosDbCollectionNameMessages
            {
                get
                {
                    return ConfigurationManager.AppSettings["cdb:ColMessages"];
                }
            }

            public static string CosmosDbCollectionNameSettings
            {
                get
                {
                    return ConfigurationManager.AppSettings["cdb:ColSettings"];
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