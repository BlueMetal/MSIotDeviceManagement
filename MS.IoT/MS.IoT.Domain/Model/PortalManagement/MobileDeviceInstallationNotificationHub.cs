using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MS.IoT.Domain.Model
{
    public class MobileDeviceInstallationNotificationHub
    {
        public MobileDeviceInstallationNotificationHub()
        {
            Tags = new List<string>();
            Templates = new Dictionary<string, NotificationPushTemplate>();
        }
        [JsonProperty(PropertyName = "installationId")]
        public string InstallationId { get; set; }

        [JsonProperty(PropertyName = "platform")]
        public string Platform { get; set; }

        [JsonProperty(PropertyName = "pushChannel")]
        public string PushChannel { get; set; }

        [JsonProperty(PropertyName = "tags")]
        public List<string> Tags { get; set; }

        [JsonProperty(PropertyName = "templates")]
        public Dictionary<string, NotificationPushTemplate> Templates { get; set; }
    }
}
