using Microsoft.Azure.ActiveDirectory.GraphClient;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MS.IoT.Domain.Model
{
    public class UpdateApplicationPasswordCredsRequest
    {
        [JsonProperty(PropertyName = "value")]
        public List<UpdateApplicationPasswordCredentials> UpdateApplicationPasswordCreds { get; set; }  
    }

    public class UpdateApplicationPasswordCredentials
    {
        [JsonProperty(PropertyName = "startDate")]
        public DateTime StartDate { get; set; }

        [JsonProperty(PropertyName = "endDate")]
        public DateTime EndDate { get; set; }

        [JsonProperty(PropertyName = "keyId")]
        public Guid KeyId { get; set; }

        [JsonProperty(PropertyName = "value")]
        public string Value { get; set; }
    }
}
