using Newtonsoft.Json;

namespace MS.IoT.Domain.Model
{
    public class DeviceFeature
    {
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "isActivated")]
        public bool IsActivated { get; set; }
    }
}
