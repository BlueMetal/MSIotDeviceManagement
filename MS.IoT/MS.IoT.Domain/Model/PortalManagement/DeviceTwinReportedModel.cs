using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace MS.IoT.Domain.Model
{ 
    public class DeviceTwinReportedModel
    {
        [JsonProperty(PropertyName = "statusCode")]
        public int StatusCode { get; set; }

        [JsonProperty(PropertyName = "firmwareVersion")]
        public string FirmwareVersion { get; set; }

        [JsonProperty(PropertyName = "heartbeat")]
        public DateTime? Heartbeat { get; set; }

        [JsonProperty(PropertyName = "activationDate")]
        public DateTime? ActivationDate { get; set; }

        [JsonProperty(PropertyName = "ipAddress")]
        public string IpAddress { get; set; }

        [JsonProperty(PropertyName = "featuresDefinitions")]
        public IDictionary<string,DeviceTwinReportedFeaturesModel> Features { get; set; }
    }
}
