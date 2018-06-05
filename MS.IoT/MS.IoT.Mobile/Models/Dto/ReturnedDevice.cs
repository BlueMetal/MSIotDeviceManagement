using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace MS.IoT.Mobile
{
    public class ReturnedDevice
    {
        [JsonProperty(PropertyName = "deviceId")]
        public string DeviceId { get; set; }

        [JsonProperty(PropertyName = "state")]
        public int State { get; set; }

        [JsonProperty(PropertyName = "tags")]
        public Tags Tags { get; set; }

        [JsonProperty(PropertyName = "desired")]
        public object Desired { get; set; }

        [JsonProperty(PropertyName = "reported")]
        public Reported Reported { get; set; }

    }

    public class Location
    {
        [JsonProperty(PropertyName = "ip")]
        public string Ip { get; set; }

        [JsonProperty(PropertyName = "country_code")]
        public string CountryCode { get; set; }

        [JsonProperty(PropertyName = "country_name")]
        public string CountryName { get; set; }

        [JsonProperty(PropertyName = "region_code")]
        public string Regioncode { get; set; }

        [JsonProperty(PropertyName = "region_name")]
        public string RegionName { get; set; }

        [JsonProperty(PropertyName = "city")]
        public string City { get; set; }

        [JsonProperty(PropertyName = "zip_code")]
        public string Zipcode { get; set; }

        [JsonProperty(PropertyName = "time_zone")]
        public string TimeZone { get; set; }

        [JsonProperty(PropertyName = "latitude")]
        public string Latitude { get; set; }

        [JsonProperty(PropertyName = "longitude")]
        public string Longitude { get; set; }

        [JsonProperty(PropertyName = "metro_code")]
        public string MetroCode { get; set; }
    }

    public class Tags
    {
        [JsonProperty(PropertyName = "productName")]
        public string ProductName { get; set; }

        [JsonProperty(PropertyName = "productFamily")]
        public string ProductFamily { get; set; }

        [JsonProperty(PropertyName = "productType")]
        public string ProductType{ get; set; }

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
        public Location Location { get; set; }

        [JsonProperty(PropertyName = "demo")]
        public bool Demo { get; set; }
    }




    public class Reported
    {
        [JsonProperty(PropertyName = "statusCode")]
        public int StatusCode { get; set; }

        [JsonProperty(PropertyName = "firmwareVersion")]
        public double FirmwareVersion { get; set; }

        [JsonProperty(PropertyName = "heartbeat")]
        public DateTime? Heartbeat { get; set; }

        [JsonProperty(PropertyName = "activationDate")]
        public DateTime? ActivationDate { get; set; }

        [JsonProperty(PropertyName = "ipAddress")]
        public string IpAddress { get; set; }

        [JsonProperty(PropertyName = "featuresDefinitions")]
        public IDictionary<string, FeatureDefinition> Features { get; set; }

    }


}
