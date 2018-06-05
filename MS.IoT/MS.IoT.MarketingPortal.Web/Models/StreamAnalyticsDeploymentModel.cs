using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MS.IoT.MarketingPortal.Web.Models
{
    public class StreamAnalyticsDeploymentModel
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
