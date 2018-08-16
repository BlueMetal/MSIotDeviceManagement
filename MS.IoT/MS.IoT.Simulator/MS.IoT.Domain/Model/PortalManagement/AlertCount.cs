using Newtonsoft.Json;

namespace MS.IoT.Domain.Model
{
    public class AlertCount
    {
        [JsonProperty(PropertyName = "alert")]
        public int AlertCode { get; set; }

        [JsonProperty(PropertyName = "count")]
        public int Count { get; set; }
    }
}
