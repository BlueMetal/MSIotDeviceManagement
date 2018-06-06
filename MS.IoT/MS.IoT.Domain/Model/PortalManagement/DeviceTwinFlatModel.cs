using MS.IoT.Domain.Model;
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
    public class DeviceTwinFlatModel
    {
        [JsonProperty(PropertyName = "deviceId")]
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
        public DateTime ManufacturedDate { get; set; }

        [JsonProperty(PropertyName = "retailerName")]
        public string RetailerName { get; set; }

        [JsonProperty(PropertyName = "retailerRegion")]
        public string RetailerRegion { get; set; }

        [JsonProperty(PropertyName = "shippedDate")]
        public DateTime ShippedDate { get; set; }

        [JsonProperty(PropertyName = "location")]
        public LocationAddress Location { get; set; }

        [JsonProperty(PropertyName = "customTags")]
        public IDictionary<string, string> CustomTags { get; set; }

        [JsonProperty(PropertyName = "statusCode")]
        public int StatusCode { get; set; }

        [JsonProperty(PropertyName = "firmwareVersion")]
        public string FirmwareVersion { get; set; }

        //[JsonProperty(PropertyName = "heartbeat")]
        //public DateTime Heartbeat { get; set; }

        [JsonProperty(PropertyName = "activationDate")]
        public DateTime ActivationDate { get; set; }

        [JsonProperty(PropertyName = "ipAddress")]
        public string IpAddress { get; set; }

        [JsonProperty(PropertyName = "deviceState")]
        public IDictionary<string, string> DeviceState { get; set; }

        [JsonProperty(PropertyName = "connectionState")]
        public DeviceConnectionStatus ConnectionState { get; set; }

        [JsonProperty(PropertyName = "installedLocation")]
        public string InstalledLocation
        {
            get
            {
                if (string.IsNullOrEmpty(Location.City) && string.IsNullOrEmpty(Location.RegionCode))
                    return string.Empty;
                if (string.IsNullOrEmpty(Location.City))
                    return Location.RegionCode;
                if (string.IsNullOrEmpty(Location.RegionCode))
                    return Location.City;
                return Location.RegionCode + ", " + Location.City;
            }
        }
    }
}
