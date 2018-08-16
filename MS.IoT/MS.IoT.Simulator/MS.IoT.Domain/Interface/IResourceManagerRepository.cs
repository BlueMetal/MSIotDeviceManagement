using Microsoft.Azure.Management.ResourceManager.Models;
using Microsoft.Rest;
using MS.IoT.Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace MS.IoT.Domain.Interface
{
    public interface IResourceManagerRepository
    {
        Task<SubscriptionResponse> GetSubscriptions(string token);

        Task<LocationResponse> GetLocations(string subscriptionId, string token);

        Task<ResourceGroup> CreateResoureGroup(string subscriptionId, string location, string resourceGroupName, string token);

        Task<DeploymentExtended> Deploy4x4MSIoTSolutionUsingAzureRMTemplate(DeploymentRequest depReq, string token);

        Task<DeploymentExtended> GetDeploymentStatus(DeploymentStatusRequest deployStatus, string token);

        Task<DeploymentValidateResult> Validate4x4MSIoTSolutionUsingAzureRMTemplate(DeploymentRequest depReq, string token);

        Task<DeploymentExtended> Deploy4x4StreamAnalyticsUsingAzureRMTemplate(StreamAnalyticsDeploymentRequest depReq, string token);

        Task<DeploymentValidateResult> Validate4x4StreamAnalyticsUsingAzureRMTemplate(StreamAnalyticsDeploymentRequest depReq, string token);
    }
}
