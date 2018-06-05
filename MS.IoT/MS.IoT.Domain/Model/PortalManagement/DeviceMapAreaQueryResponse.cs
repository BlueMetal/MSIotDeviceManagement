using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MS.IoT.Domain.Model
{
    public class DeviceMapAreaQueryResponse
    {
        [JsonProperty(PropertyName = "areaItems")]
        public Dictionary<string, int> AreaItems { get; set; }

        [JsonProperty(PropertyName = "isDatabaseLoaded")]
        public bool IsDatabaseLoaded { get; set; }

        [JsonProperty(PropertyName = "isDatabaseLoading")]
        public bool IsDatabaseLoading { get; set; }

        [JsonProperty(PropertyName = "lastUpdated")]
        public DateTime LastUpdate { get; set; }

        [JsonProperty(PropertyName = "errorMessage")]
        public string ErrorMessage { get; set; }

        [JsonProperty(PropertyName = "success")]
        public bool Success { get; set; }

        public DeviceMapAreaQueryResponse()
        {
            AreaItems = new Dictionary<string, int>();
        }
    }
}
