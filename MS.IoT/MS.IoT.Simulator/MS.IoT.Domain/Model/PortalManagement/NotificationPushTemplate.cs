using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MS.IoT.Domain.Model
{
    public class NotificationPushTemplate
    {
        [JsonProperty(PropertyName = "body")]
        public string Body { get; set; }
    }
}
