using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MS.IoT.Domain.Model
{
    public class DeploymentStatusRequest
    {
        public string ApplicationName { get; set; }      
        public string SubscriptionId { get; set; }      
    }
}
