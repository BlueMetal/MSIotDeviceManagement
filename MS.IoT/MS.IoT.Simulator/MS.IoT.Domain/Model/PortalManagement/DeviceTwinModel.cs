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
    public class DeviceTwinModel
    {
        [JsonProperty(PropertyName = "deviceId")]
        public string DeviceId { get; set; }

        [JsonProperty(PropertyName = "tags")]
        public DeviceTwinTagsModel Tags { get; set; }

        [JsonProperty(PropertyName = "desired")]
        public DeviceTwinDesiredModel Desired { get; set; }

        [JsonProperty(PropertyName = "reported")]
        public DeviceTwinReportedModel Reported { get; set; }
    }
}
