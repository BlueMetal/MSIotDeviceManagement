using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MS.IoT.Domain.Model
{
    public class StreamAnalyticsDeploymentRequest
    {
        public string IoTHubName { get; set; }
        public string CosmosDBAccountName { get; set; }
        public string CosmosDBName { get; set; }
        public string CosmosDBMessageCollectionName { get; set; }
        public string ResourceGroupName { get; set; }
        public string SubscriptionId { get; set; }
        public string Location { get; set; }
    }
}
