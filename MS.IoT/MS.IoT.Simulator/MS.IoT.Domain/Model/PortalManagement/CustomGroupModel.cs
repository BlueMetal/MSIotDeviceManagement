using Newtonsoft.Json;

namespace MS.IoT.Domain.Model
{
    public class CustomGroupModel
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "where")]
        public DeviceQueryRuleGroup Where { get; set; }

        [JsonProperty(PropertyName = "count")]
        public int Count { get; set; }

        [JsonProperty(PropertyName = "order")]
        public int Order { get; set; }
    }
}
