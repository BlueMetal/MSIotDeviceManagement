using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.Management.ResourceManager;
using Microsoft.Rest;
using MS.IoT.Domain.Interface;

namespace MS.IoT.Repositories
{
    public class ArmClientFactory : IArmClientFactory
    {
        private readonly Func<Task<string>> accessTokenGetter;
        private readonly string endpointAddress;

        public ArmClientFactory(Func<Task<string>> accessTokenGetter, string endpointAddress)
        {
            this.accessTokenGetter = accessTokenGetter;
            this.endpointAddress = endpointAddress;
        }
        public async Task<ResourceManagementClient> GetResourceMangementClientAsync(string subscriptionId)
        {
            var creds = new TokenCredentials(await accessTokenGetter());
            return new ResourceManagementClient(new Uri(endpointAddress), creds)
            {
                SubscriptionId = subscriptionId
            };
        }

        public async Task<SubscriptionClient> GetSubscriptionClientAsync()
        {
            var creds = new TokenCredentials(await accessTokenGetter());
            return new SubscriptionClient(new Uri(endpointAddress), creds);
        }
    }
}
