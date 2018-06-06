using MS.IoT.Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace MS.IoT.Domain.Interface
{
    public interface IServicePrincipalRepository
    {
        Task<ServicePrincipalResponse> CreateAppAndServicePrincipal(string displayName, string appIdUri, string password, string tenantId);

        Task UpdateAzureADApplication(string appObjectId, UpdateApplicationRequest updateReq, string tenantId);

        Task UpdateAzureADApplicationPasswordCredentials(string appObjectId, UpdateApplicationPasswordCredsRequest updateReq, string tenantId);
    }
}
