using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MS.IoT.Domain.Model
{
    public class DeviceQueryConfiguration
    {
        [JsonProperty(PropertyName = "pageIndex")]
        public int PageIndex { get; set; }

        [JsonProperty(PropertyName = "itemsPerPage")]
        public int ItemsPerPage { get; set; }

        [JsonProperty(PropertyName = "orderBy")]
        public string OrderBy { get; set; }

        [JsonProperty(PropertyName = "orderBySorting")]
        public OrderBySort OrderBySorting { get; set; }

        [JsonProperty(PropertyName = "groupBy")]
        public string GroupBy { get; set; }

        [JsonProperty(PropertyName = "where")]
        public DeviceQueryRuleGroup Where { get; set; }

        public DeviceQueryConfiguration()
        {
        }
    }
}
