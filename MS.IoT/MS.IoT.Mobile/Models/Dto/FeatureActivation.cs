using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace MS.IoT.Mobile
{
    public class FeatureActivation
    {
        [JsonProperty(PropertyName = "deviceId")]
        public string DeviceId;

        [JsonProperty(PropertyName = "feature")]
        public DtoFeature Feature;
    }

    public class DtoFeature
    {
        [JsonProperty(PropertyName = "name")]
        public string Name;

        [JsonProperty(PropertyName = "isActivated")]
        public bool IsActivated;
    }

}
