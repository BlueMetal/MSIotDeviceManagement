using Newtonsoft.Json;
using System.Collections.Generic;

namespace MS.IoT.Domain.Model
{ 
    public class DeviceTwinUpdateAsyncModel
    {
        [JsonProperty(PropertyName = "deviceIds")]
        public List<string> DeviceIds { get; set; }

        [JsonProperty(PropertyName = "jsonDesired")]
        public string JsonDesired { get; set; }

        [JsonProperty(PropertyName = "jsonTags")]
        public string JsonTags { get; set; }
    }
}
