using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MS.IoT.Domain.Model
{
    public class DeviceMapGroupsInformation
    {
        [JsonProperty(PropertyName = "retailers")]
        public List<MapFilterItem> Retailers { get; set; }

        [JsonProperty(PropertyName = "productFamilies")]
        public List<MapFilterItem> ProductFamilies { get; set; }

        [JsonProperty(PropertyName = "connectionStates")]
        public List<MapFilterItem> ConnectionStates { get; set; }

        [JsonProperty(PropertyName = "count")]
        public int ItemsCount { get; set; }

        public DeviceMapGroupsInformation()
        {
            Retailers = new List<MapFilterItem>();
            ProductFamilies = new List<MapFilterItem>();
            ConnectionStates = new List<MapFilterItem>();
        }
    }
}
