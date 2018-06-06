using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace MS.IoT.Domain.Model
{ 
    public class DeviceTwinImportModel
    {
        [JsonIgnore]
        public string DeviceId { get; set; }

        [JsonProperty(PropertyName = "productName")]
        public string ProductName { get; set; }

        [JsonProperty(PropertyName = "productType")]
        public string ProductType { get; set; }

        [JsonProperty(PropertyName = "productFamily")]
        public string ProductFamily { get; set; }

        [JsonProperty(PropertyName = "userId")]
        public string UserId { get; set; }

        [JsonProperty(PropertyName = "manufacturedDate")]
        public DateTime? ManufacturedDate { get; set; }

        [JsonProperty(PropertyName = "retailerName")]
        public string RetailerName { get; set; }

        [JsonProperty(PropertyName = "retailerRegion")]
        public string RetailerRegion { get; set; }

        [JsonProperty(PropertyName = "shippedDate")]
        public DateTime? ShippedDate { get; set; }
    }
}
