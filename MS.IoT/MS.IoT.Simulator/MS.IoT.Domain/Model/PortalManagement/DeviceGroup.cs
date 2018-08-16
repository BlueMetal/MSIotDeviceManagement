using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MS.IoT.Domain.Model
{
    public class DeviceGroup
    {
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "itemsCount")]
        public int ItemsCount { get; set; }

        [JsonProperty(PropertyName = "itemsIds")]
        public IEnumerable<string> ItemsIds { get; set; }
    }
}
