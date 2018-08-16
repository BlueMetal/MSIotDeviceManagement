using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MS.IoT.Domain.Model
{
    public class DeviceQueryResponse
    {
        [JsonProperty(PropertyName = "items")]
        public IEnumerable<DeviceInfoEntity> Items { get; set; }

        [JsonProperty(PropertyName = "groups")]
        public IEnumerable<DeviceGroup> Groups { get; set; }

        [JsonProperty(PropertyName = "itemsCount")]
        public int ItemsCount { get; set; }

        [JsonProperty(PropertyName = "groupsCount")]
        public int GroupsCount { get; set; }

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

        public DeviceQueryResponse()
        {
            Items = new List<DeviceInfoEntity>();
            Groups = new List<DeviceGroup>();
        }
    }
}
