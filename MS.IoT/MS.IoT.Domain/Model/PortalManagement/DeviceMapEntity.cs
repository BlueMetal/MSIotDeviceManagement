using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MS.IoT.Domain.Model
{
    public class DeviceMapEntity
    {
        //[JsonProperty(PropertyName = "name")]
        //public string Name { get; set; }

        [JsonProperty(PropertyName = "lon")]
        public string GeoLongitude { get; set; }

        [JsonProperty(PropertyName = "lat")]
        public string GeoLatitude { get; set; }

        //[JsonProperty(PropertyName = "count")]
        //public int Count { get; set; }
    }
}
