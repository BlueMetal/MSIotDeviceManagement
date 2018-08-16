using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MS.IoT.Domain.Model
{
    public class DirectMethodResponse
    {
        [JsonProperty(PropertyName = "status")]
        public string Status { get; set; }

        [JsonProperty(PropertyName = "newValue")]
        public int NewValue { get; set; }

        [JsonProperty(PropertyName = "timeTillCompletion")]
        public DateTime TimeTillCompletion { get; set; }
    }
}
