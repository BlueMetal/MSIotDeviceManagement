using System;
using Newtonsoft.Json;

namespace MS.IoT.Mobile.Services.Notifications
{ 
    public class PushRequest
    {
        [JsonProperty("text")]
        public string Text { get; set; }

        [JsonProperty("tags")]
        public string[] Tags { get; set; }

        [JsonProperty("silent")]
        public bool Silent { get; set; }

        [JsonProperty("action")]
        public string Action { get; set; }
    }
}
