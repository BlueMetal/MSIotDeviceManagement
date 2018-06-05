using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MS.IoT.Domain.Model
{
    public class DeviceTwinRetailerRegionCount
    {
        [JsonProperty(PropertyName = "deviceCount")]
        public int DeviceCount { get; set; }

        [JsonProperty(PropertyName = "retailerRegion")]
        public string RetailerRegion { get; set; }
    }
}
