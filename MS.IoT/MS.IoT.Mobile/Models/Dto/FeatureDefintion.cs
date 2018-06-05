using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace MS.IoT.Mobile
{
    public class FeatureDefinition
    {
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "displayName")]
        public string DisplayName { get; set; }

        [JsonProperty(PropertyName = "methods")]
        public string MethodNames { get; set; }

        [JsonProperty(PropertyName = "isActivated")]
        public bool IsActivated { get; set; }

        [JsonProperty(PropertyName = "internalUseOnly")]
        public bool IsInternal { get; set; }
    }
}
