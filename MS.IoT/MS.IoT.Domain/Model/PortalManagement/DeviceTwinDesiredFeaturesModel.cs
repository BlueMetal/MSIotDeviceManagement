using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MS.IoT.Domain.Model
{
    public class DeviceTwinDesiredFeaturesModel
    {
        [JsonProperty(PropertyName = "deviceId")]
        public string DeviceId { get; set; }

        [JsonProperty(PropertyName = "features")]
        public List<DeviceFeature> Features { get; set; }
    }
}
