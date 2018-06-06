using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using MS.IoT.DataPacketDesigner.Web.Models;

namespace MS.IoT.DataPacketDesigner.Web.Helpers
{
    public class TokenServices : ITokenServices
    {
        private readonly AuthenticationContext authenticationContext;
        private readonly IOptionsSnapshot<AzureAdOptions> azureAdOptions;
        private readonly IOptionsSnapshot<ResourceIds> resourceIds;
        private readonly IHttpContextAccessor httpContextAccessor;

        public TokenServices(AuthenticationContext authenticationContext, 
                             IOptionsSnapshot<AzureAdOptions> azureAdOptions,
                             IOptionsSnapshot<ResourceIds> resourceIds,
                             IHttpContextAccessor httpContextAccessor)
        {
            this.authenticationContext = authenticationContext;
            this.azureAdOptions = azureAdOptions;
            this.resourceIds = resourceIds;
            this.httpContextAccessor = httpContextAccessor;
        }

        public async Task<string> GetAzureManagementTokenAsync()
        {
            var res = await authenticationContext.AcquireTokenSilentAsync(resourceIds.Value.AzureRmId, new ClientCredential(azureAdOptions.Value.ClientId, azureAdOptions.Value.ClientSecret), UserIdentifier.AnyUser);
            return res.AccessToken;
        }

        public async Task<string> GetGraphTokenAsync()
        {
            var res = await authenticationContext.AcquireTokenSilentAsync(resourceIds.Value.GraphId, new ClientCredential(azureAdOptions.Value.ClientId, azureAdOptions.Value.ClientSecret), UserIdentifier.AnyUser);
            return res.AccessToken;
        }

        public string TenantId => httpContextAccessor.HttpContext.User.FindFirst("tid")?.Value;
        public string DirectoryInstance => $"{azureAdOptions.Value.GraphEndpoint}/{TenantId}";
    }
}

