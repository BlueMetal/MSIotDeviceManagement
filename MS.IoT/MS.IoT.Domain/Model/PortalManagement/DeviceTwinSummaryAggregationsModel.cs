using MS.IoT.Domain.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MS.IoT.Domain.Model
{
    public class DeviceTwinSummaryAggregationsModel
    {
        [JsonProperty(PropertyName = "totalDevicesCount")]
        public int TotalDevicesCount { get; set; }

        [JsonProperty(PropertyName = "activatedDevicesCount")]
        public int ActivatedDevicesCount { get; set; }

        [JsonProperty(PropertyName = "notActivatedDevicesCount")]
        public int NotActivatedDevicesCount { get; set; }

        [JsonProperty(PropertyName = "connectedDevicesCount")]
        public int ConnectedDevicesCount { get; set; }

        [JsonProperty(PropertyName = "disconnectedDevicesCount")]
        public int DisconnectedDevicesCount { get; set; }

        [JsonProperty(PropertyName = "alertCounts")]
        public List<AlertCount> AlertCounts { get; set; }

        [JsonProperty(PropertyName = "devicePerGroupActivated")]
        public List<DevicePerGroupActivated> DevicePerGroupActivated { get; set; }
    }
}
