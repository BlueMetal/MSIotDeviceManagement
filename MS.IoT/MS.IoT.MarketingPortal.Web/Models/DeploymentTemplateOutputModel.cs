using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MS.IoT.MarketingPortal.Web.Models
{
    public class DeploymentTemplateOutputModel
    {
        public Properties properties { get; set; }
        
        [JsonProperty(PropertyName = "dataPacketDesignerUrl")]
        public OutputValue DataPacketDesignerURL { get; set; }

        [JsonProperty(PropertyName = "deviceManagementPortalUrl")]
        public OutputValue DeviceManagementPortalUrl { get; set; }

        [JsonProperty(PropertyName = "iotHubName")]
        public OutputValue IotHubName { get; set; }

        [JsonProperty(PropertyName = "cosmosDBAccountName")]
        public OutputValue CosmosDBAccountName { get; set; }

        [JsonProperty(PropertyName = "cosmosDBAccountEndPoint")]
        public OutputValue CosmosDBAccountEndPoint { get; set; }

        [JsonProperty(PropertyName = "cosmosDBAccountKey")]
        public OutputValue CosmosDBAccountKey { get; set; }
    }


    public class Properties
    {
        public string provisioningState { get; set; }
    }

    public class OutputValue
    {
        [JsonProperty(PropertyName = "type")]
        public string Type { get; set; }

        [JsonProperty(PropertyName = "value")]
        public string Value { get; set; }
    }
}