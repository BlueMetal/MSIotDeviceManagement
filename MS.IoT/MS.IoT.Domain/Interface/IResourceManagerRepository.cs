using System.Collections.Generic;
using Microsoft.Azure.Management.ResourceManager.Models;
using MS.IoT.Domain.Model;
using System.Threading.Tasks;

namespace MS.IoT.Domain.Interface
{
    public interface IResourceManagerRepository
    {
        Task<IReadOnlyList<Subscription>> GetSubscriptions();

        Task<IReadOnlyList<Location>> GetLocations(string subscriptionId);

        Task<ResourceGroup> CreateResoureGroup(string subscriptionId, string location, string resourceGroupName);

        Task<DeploymentExtended> Deploy4x4MSIoTSolutionUsingAzureRMTemplate(DeploymentRequest depReq);

        Task<DeploymentExtended> GetDeploymentStatus(DeploymentStatusRequest deployStatus);

        Task<DeploymentValidateResult> Validate4x4MSIoTSolutionUsingAzureRMTemplate(DeploymentRequest depReq);

        Task<DeploymentExtended> Deploy4x4StreamAnalyticsUsingAzureRMTemplate(StreamAnalyticsDeploymentRequest depReq);

        Task<DeploymentValidateResult> Validate4x4StreamAnalyticsUsingAzureRMTemplate(StreamAnalyticsDeploymentRequest depReq);
    }
}
