using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.Management.ResourceManager;

namespace MS.IoT.Domain.Interface
{
    public interface IArmClientFactory
    {
        Task<SubscriptionClient> GetSubscriptionClientAsync();
        Task<ResourceManagementClient> GetResourceMangementClientAsync(string subscriptionId);
    }
}
