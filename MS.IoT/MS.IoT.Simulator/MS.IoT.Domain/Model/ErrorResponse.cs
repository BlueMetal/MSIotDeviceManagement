using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MS.IoT.Domain.Model
{
    public class ErrorResponse
    {
        [JsonProperty(PropertyName = "error")]
        public Error Error { get; set; }
    }
    public class Error
    {     
        [JsonProperty(PropertyName = "code")]
        public string Code { get; set; }
        [JsonProperty(PropertyName = "message")]
        public string Message { get; set; }  
    }
}
