using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace MS.IoT.Mobile
{
    public class InvokeFeatureResponse
    {
        [JsonProperty(PropertyName = "status")]
        public string Status { get; set; }

        [JsonProperty(PropertyName = "newValue")]
        public int NewValue { get; set; }

        [JsonProperty(PropertyName = "timeTillCompletion")]
        public DateTime TimeTillCompletion { get; set; }



    }
}
