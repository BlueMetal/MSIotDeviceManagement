using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MS.IoT.Domain.Model
{
    public class DeviceInfoEntity
    {
        [JsonProperty(PropertyName = "deviceId")]
        public string DeviceId { get; set; }

        [JsonProperty(PropertyName = "productName")]
        public string ProductName { get; set; }

        [JsonProperty(PropertyName = "productFamily")]
        public string ProductFamily { get; set; }

        [JsonProperty(PropertyName = "connectionState")]
        public DeviceConnectionStatus ConnectionState { get; set; }

        [JsonProperty(PropertyName = "retailerName")]
        public string RetailerName { get; set; }

        [JsonProperty(PropertyName = "retailerRegion")]
        public string RetailerRegion { get; set; }

        [JsonProperty(PropertyName = "statusCode")]
        public int StatusCode { get; set; }

        [JsonProperty(PropertyName = "productRegion")]
        public string ProductRegion { get; set; }

        [JsonProperty(PropertyName = "productCity")]
        public string ProductCity { get; set; }

        [JsonProperty(PropertyName = "productCountry")]
        public string ProductCountry { get; set; }

        [JsonProperty(PropertyName = "installedLocation")]
        public string InstalledLocation {
            get {
                if (string.IsNullOrEmpty(ProductCity) && string.IsNullOrEmpty(ProductRegion))
                    return string.Empty;
                if (string.IsNullOrEmpty(ProductCity))
                    return ProductRegion;
                if (string.IsNullOrEmpty(ProductRegion))
                    return ProductCity;
                return ProductRegion + ", " + ProductCity;
            }
        }

        public DeviceInfoEntity(DeviceTwinFlatModel source)
        {
            this.DeviceId = source.DeviceId;
            this.ProductName = source.ProductName;
            this.ProductFamily = source.ProductFamily;
            this.RetailerName = source.RetailerName;
            this.RetailerRegion = source.RetailerRegion;
            this.StatusCode = source.StatusCode;
            this.ConnectionState = source.ConnectionState;
            if (source.Location != null)
            {
                this.ProductRegion = source.Location.RegionCode;
                this.ProductCity = source.Location.City;
                this.ProductCountry = source.Location.CountryName;
            }         
        }
    }
}
