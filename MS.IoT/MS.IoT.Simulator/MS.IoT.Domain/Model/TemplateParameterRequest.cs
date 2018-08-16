using Microsoft.Azure.ActiveDirectory.GraphClient;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MS.IoT.Domain.Model
{
    public class TemplateParameterRequest
    {
        [JsonProperty(PropertyName = "$schema")]
        public string Schema { get; set; } = "https://schema.management.azure.com/schemas/2015-01-01/deploymentParameters.json#";

        [JsonProperty(PropertyName = "contentVersion")]
        public string contentVersion { get; set; } = "1.0.0.0";

        [JsonProperty(PropertyName = "parameters")]
        public ParameterModel Parameters { get; set; }
    }

    public class ParameterModel
    {
        [JsonProperty(PropertyName = "clientId")]
        public ParameterValue ClientId { get; set; }

        [JsonProperty(PropertyName = "clientSecret")]
        public ParameterValue ClientSecret { get; set; }

        [JsonProperty(PropertyName = "tenantId")]
        public ParameterValue TenantId { get; set; }

        [JsonProperty(PropertyName = "dataPacketDesignerpPackageWebZipUri")]
        public ParameterValue DataPacketDesignerpPackageWebZipUri { get; set; }

        [JsonProperty(PropertyName = "deviceManagementPortalPackageWebZipUri")]
        public ParameterValue DeviceManagementPortalPackageWebZipUri { get; set; }
    }

    public class ParameterValue
    {
        [JsonProperty(PropertyName = "value")]
        public string Value { get; set; }
    }
}
