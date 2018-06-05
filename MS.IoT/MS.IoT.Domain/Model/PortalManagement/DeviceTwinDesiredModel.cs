using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace MS.IoT.Domain.Model
{ 
    public class DeviceTwinDesiredModel
    {
        [JsonProperty(PropertyName = "features")]
        public IDictionary<string, bool> Features { get; set; }

        [JsonProperty(PropertyName = "deviceState")]
        public IDictionary<string, object> DeviceState { get; set; }

        [JsonProperty(PropertyName = "statusCode")]
        public int StatusCode { get; set; }

        [JsonProperty(PropertyName = "firmwareVersion")]
        public string FirmwareVersion { get; set; }
    }
}
