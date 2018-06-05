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
    public class DeviceTwinReportedFeaturesModel
    {
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "displayName")]
        public string DisplayName { get; set; }

        [JsonProperty(PropertyName = "methods")]
        public string MethodName { get; set; }

        [JsonProperty(PropertyName = "isActivated")]
        public bool IsActivated { get; set; }

        [JsonProperty(PropertyName = "internalUseOnly")]
        public bool InternalUseOnly { get; set; }
    }
}
