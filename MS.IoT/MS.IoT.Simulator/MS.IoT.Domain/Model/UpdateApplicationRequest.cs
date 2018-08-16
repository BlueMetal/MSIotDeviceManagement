using Microsoft.Azure.ActiveDirectory.GraphClient;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MS.IoT.Domain.Model
{
    public class UpdateApplicationRequest
    {
        [JsonProperty(PropertyName = "replyUrls")]
        public List<string> ReplyUrls { get; set; }

        [JsonProperty(PropertyName = "homepage")]
        public string Homepage { get; set; }     
    }  
}
