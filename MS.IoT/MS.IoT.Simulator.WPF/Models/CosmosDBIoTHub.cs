using Newtonsoft.Json;
using System;

namespace MS.IoT.Simulator.WPF.Models
{
    /// <summary>
    /// CosmosDBIoTHub
    /// Contains a mapping of the property of the node IoTHub added by IoT Hub while transmitting to stream analytics
    /// </summary>
    public class CosmosDBIoTHub
    {
        [JsonProperty(PropertyName = "MessageId")]
        public string MessageId { get; set; }
        [JsonProperty(PropertyName = "CorrelationId")]
        public string CorrelationId { get; set; }
        [JsonProperty(PropertyName = "ConnectionDeviceId")]
        public string ConnectionDeviceId { get; set; }
        [JsonProperty(PropertyName = "ConnectionDeviceGenerationId")]
        public string ConnectionDeviceGenerationId { get; set; }
        [JsonProperty(PropertyName = "EnqueuedTime")]
        public DateTime EnqueuedTime { get; set; }
        [JsonProperty(PropertyName = "StreamId")]
        public string StreamId { get; set; }
    }
}
