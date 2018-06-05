using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace MS.IoT.Simulator.WPF.Models
{
    /// <summary>
    /// TemplatePacket
    /// Object generated with TemplateInstanceHelper with the help of a Template. This object contains an implementation of Template simulating real situation usage.
    /// </summary>
    public class TemplatePacket
    {
        [JsonProperty(PropertyName = "demoId")]
        public int DemoId { get; set; }

        [JsonProperty(PropertyName = "demoInstanceId")]
        public Guid DemoInstanceId { get; set; }

        [JsonProperty(PropertyName = "templateId")]
        public string TemplateId { get; set; }

        [JsonProperty(PropertyName = "properties")]
        public Dictionary<string, dynamic> Properties { get; set; }
    }
}
