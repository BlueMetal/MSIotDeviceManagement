using Newtonsoft.Json;

namespace MS.IoT.Domain.Model
{ 
    public class DeviceTwinUpdateSyncModel
    {
        [JsonProperty(PropertyName = "deviceId")]
        public string DeviceId { get; set; }

        [JsonProperty(PropertyName = "jsonDesired")]
        public string JsonDesired { get; set; }

        [JsonProperty(PropertyName = "jsonTags")]
        public string JsonTags { get; set; }
    }
}
