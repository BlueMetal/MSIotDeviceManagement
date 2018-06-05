using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace MS.IoT.Mobile
{
    public class PushNotificationDto
    {
        [JsonProperty(PropertyName = "notificationType")]
        public string NotificationType { get; set; }

        [JsonProperty(PropertyName = "id")]
        public string FeatureId { get; set; }
        
        [JsonProperty(PropertyName = "featureType")]
        public string FeatureType { get; set; }

        [JsonProperty(PropertyName = "subType")]
        public string SubType { get; set; }

        [JsonProperty(PropertyName = "deviceId")]
        public string DeviceId { get; set; }

        [JsonProperty(PropertyName = "featureName")]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "featureDescription")]
        public string Description { get; set; }

        [JsonProperty(PropertyName = "methodName")]
        public string MethodName { get; set; }

        [JsonProperty(PropertyName = "message")]
        public string Message { get; set; }

        [JsonProperty(PropertyName = "url")]
        public string LinkUrl { get; set; }

        [JsonProperty(PropertyName = "customIconName")]
        public string CustomIcon { get; set; }

    }
}
