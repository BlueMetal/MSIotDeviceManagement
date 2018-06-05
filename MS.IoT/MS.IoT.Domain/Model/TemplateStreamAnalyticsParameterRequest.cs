using Microsoft.Azure.ActiveDirectory.GraphClient;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MS.IoT.Domain.Model
{
    public class TemplateStreamAnalyticsParameterRequest
    {
        [JsonProperty(PropertyName = "$schema")]
        public string Schema { get; set; } = "https://schema.management.azure.com/schemas/2015-01-01/deploymentParameters.json#";

        [JsonProperty(PropertyName = "contentVersion")]
        public string contentVersion { get; set; } = "1.0.0.0";

        [JsonProperty(PropertyName = "parameters")]
        public ParameterStreamAnalyticsModel Parameters { get; set; }
    }

    public class ParameterStreamAnalyticsModel
    {
        [JsonProperty(PropertyName = "iotHubName")]
        public ParameterStreamAnalyticsValue IotHubName { get; set; }

        [JsonProperty(PropertyName = "cosmosDbAccountName")]
        public ParameterStreamAnalyticsValue CosmosDbAccountName { get; set; }

        [JsonProperty(PropertyName = "cosmosDbName")]
        public ParameterStreamAnalyticsValue CosmosDbName { get; set; }

        [JsonProperty(PropertyName = "cosmosDBMessageCollectionName")]
        public ParameterStreamAnalyticsValue CosmosDBMessageCollectionName { get; set; }
    }

    public class ParameterStreamAnalyticsValue
    {
        [JsonProperty(PropertyName = "value")]
        public string Value { get; set; }
    }
}
