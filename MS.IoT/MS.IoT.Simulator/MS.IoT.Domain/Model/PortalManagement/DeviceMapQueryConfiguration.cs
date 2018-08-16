using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MS.IoT.Domain.Model
{
    public class DeviceMapQueryConfiguration
    {
        [JsonProperty(PropertyName = "viewId")]
        public string ViewId { get; set; }

        [JsonProperty(PropertyName = "filters")]
        public List<DeviceQueryRule> Filters { get; set; }

        public DeviceMapQueryConfiguration()
        {
            Filters = new List<DeviceQueryRule>();
        }
    }
}
