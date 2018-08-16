using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MS.IoT.Domain.Model
{
    public class DevicePerGroupActivated
    {
        [JsonProperty(PropertyName = "name")]
        public string GroupName { get; set; }

        [JsonProperty(PropertyName = "percentageActivated")]
        public int PercentageActivated { get; set; }
    }
}
