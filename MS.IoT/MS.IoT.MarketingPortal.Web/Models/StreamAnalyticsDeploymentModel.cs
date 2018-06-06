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
