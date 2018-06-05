using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MS.IoT.Domain.Model
{
    public class DeviceMapQueryResponse
    {
        [JsonProperty(PropertyName = "pushpins")]
        public IEnumerable<DeviceMapEntity> Pushpins { get; set; }

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

        public DeviceMapQueryResponse()
        {
            Pushpins = new List<DeviceMapEntity>();
        }
    }
}
