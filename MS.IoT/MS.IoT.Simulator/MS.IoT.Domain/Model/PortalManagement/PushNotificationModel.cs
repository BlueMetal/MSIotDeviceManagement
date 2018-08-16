using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MS.IoT.Domain.Model
{
    public class PushNotificationModel
    {
        [JsonProperty(PropertyName = "deviceIdList")]
        public List<string> DeviceIdList { get; set; }

        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        [JsonProperty(PropertyName = "featureName")]
        public string FeatureName { get; set; }

        [JsonProperty(PropertyName = "methodName")]
        public string MethodName { get; set; }

        [JsonProperty(PropertyName = "featureDescription")]
        public string FeatureDescription { get; set; }

        [JsonProperty(PropertyName = "notificationType")]
        public string NotificationType { get; set; }

        [JsonProperty(PropertyName = "url")]
        public string LinkUrl { get; set; }

        [JsonProperty(PropertyName = "customIconName")]
        public string CustomIcon { get; set; }

    }
}
