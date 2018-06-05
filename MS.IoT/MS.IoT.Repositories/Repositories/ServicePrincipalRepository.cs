using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;
using MS.IoT.Domain.Interface;
using MS.IoT.Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Web;
using Microsoft.Azure.ActiveDirectory.GraphClient;
using System.Diagnostics;
using System.Security.Cryptography;
using RestSharp;
using Newtonsoft.Json;
using MS.IoT.Common;

namespace MS.IoT.Repositories
{

    public class ServicePrincipalRepository : IServicePrincipalRepository
    {
        private static string _graphUrl;
        public ServicePrincipalRepository(string graphUrl)
        {
            _graphUrl = graphUrl;
        }

        public async Task<ServicePrincipalResponse> CreateAppAndServicePrincipal(string displayName, string appIdUri, string password, string tenantId, string token)
        {
            try
            {
                // First create the Azure AD Graph API client proxy
                var baseUri = new Uri(_graphUrl);
                var adClient = new ActiveDirectoryClient
                                    (
                                        new Uri(baseUri, tenantId),
                                        async () =>
                                        {
                                            if (token == null)
                                                throw new Exception("Authorization required before calling into Graph!");

                                            return await Task.FromResult<string>(token);
                                        }
                                    );

                // First create the app backing up the service principal          
                var appResponse = await CreateAzureADApplicationIfNotExists(displayName, appIdUri, adClient,
                    tenantId,token);

                // Now we can create the service principal to be created in the AD
                var spResponse = await CreateServicePrincipalIfNotExists(appResponse.App, password, adClient);

                // Finally return the AppId
                return await Task.FromResult
                                (
                                    new ServicePrincipalResponse
                                    {
                                        App = appResponse.App,
                                        AppClientSecret = appResponse.AppClientSecret,
                                        IsNewApp = appResponse.IsNewApp,
                                        Principal = spResponse.Principal,
                                        IsNewPrincipal = spResponse.IsNewPrincipal
                                    }
                                );
            }
            catch (Exception ex)
            {
                Log.Error("Create Azure AD Application {@error}", ex.Message);
                throw ex;
            }
        }

        private static async Task<ServicePrincipalResponse> CreateAzureADApplicationIfNotExists(string displayName, 
            string appIdUri, ActiveDirectoryClient adClient,string tenantId,string token)
        {
            try
            {
                var isNewApp = false;
                var appCreated = default(IApplication);
                ServicePrincipalResponse spr = new ServicePrincipalResponse();

                // First check if the App exists, already
                var appFilter = adClient.Applications.Where(app => app.IdentifierUris.Any(iduri => iduri == appIdUri));
                var foundApp = await appFilter.ExecuteAsync();
                if (foundApp.CurrentPage.Count == 0)
                {
                    var newApp = new Application()
                    {
                        DisplayName = displayName,

                    };
                    newApp.IdentifierUris.Add(appIdUri);
                    newApp.PasswordCredentials.Add(
                                            new PasswordCredential
                                            {
                                                StartDate = DateTime.UtcNow,
                                                EndDate = DateTime.UtcNow.AddYears(2),
                                                Value = CreateRandomClientSecretKey(),
                                                KeyId = Guid.NewGuid(),
                                            }
                                        );

                    // set Application permissions like Azure Active Directory signin and read
                    var permissions = GetActiveDirectoryPermissions();
                    newApp.RequiredResourceAccess.Add(permissions);

                    spr.AppClientSecret = newApp.PasswordCredentials.First().Value;

                    var jsonstr = JsonConvert.SerializeObject(newApp);

                    await adClient.Applications.AddApplicationAsync(newApp);

                    appCreated = newApp;
                    isNewApp = true;
                }
                else
                {
                    appCreated = foundApp.CurrentPage.FirstOrDefault();

                    // update the Password key
                    UpdateApplicationPasswordCredentials updateAppPasswordCreds =
                        new UpdateApplicationPasswordCredentials()
                        {
                            StartDate = DateTime.UtcNow,
                            EndDate = DateTime.UtcNow.AddYears(2),
                            Value = CreateRandomClientSecretKey(),
                            KeyId = Guid.NewGuid().ToString()
                        };
                    var passwordList = new List<UpdateApplicationPasswordCredentials>();
                    passwordList.Add(updateAppPasswordCreds);

                    var updateAppPasswordReq = new UpdateApplicationPasswordCredsRequest()
                    {
                        UpdateApplicationPasswordCreds = passwordList
                    };

                    await UpdateAzureADApplicationPasswordCredentialsRest(appCreated.ObjectId,
                        updateAppPasswordReq, tenantId, token);
                    spr.AppClientSecret = updateAppPasswordCreds.Value;
                }

                spr.App = appCreated;
                spr.IsNewApp = isNewApp;
                return spr;
            }
            catch (Exception ex)
            {
                Log.Error("Create Azure AD Application {@error}", ex.Message);
                throw ex;
            }
        }

        public async Task UpdateAzureADApplication(string appObjectId, UpdateApplicationRequest updateReq, 
            string tenantId, string token)
        {
            try
            {
                var client = new RestClient(_graphUrl);
                var endPoint = String.Format("/{0}/applications/{1}", tenantId, appObjectId);
                var request = new RestRequest(endPoint, Method.PATCH);
                request.AddQueryParameter("api-version", "1.6");

                request.AddHeader("Authorization", "Bearer " + token);
                request.AddHeader("Content-Type", "application/json");

                var body = JsonConvert.SerializeObject(updateReq);
                request.AddParameter("application/json", body, ParameterType.RequestBody);

                await client.ExecuteTaskAsync(request);

            }
            catch (Exception ex)
            {
                Log.Error("Update Azure AD Application {@error}", ex.Message);
                throw ex;
            }
        }

        public async Task UpdateAzureADApplicationPasswordCredentials(string appObjectId, 
            UpdateApplicationPasswordCredsRequest updateReq, string tenantId, string token)
        {
            try
            {
                await UpdateAzureADApplicationPasswordCredentialsRest(appObjectId,
                        updateReq, tenantId, token);
            }
            catch (Exception ex)
            {
                Log.Error("Update Azure AD Application {@error}", ex.Message);
                throw ex;
            }
        }

        private static async Task UpdateAzureADApplicationPasswordCredentialsRest(string appObjectId,
            UpdateApplicationPasswordCredsRequest updateReq, string tenantId, string token)
        {
            try
            {
                var client = new RestClient(_graphUrl);
                var endPoint = String.Format("/{0}/applications/{1}/passwordCredentials", tenantId, appObjectId);
                var request = new RestRequest(endPoint, Method.PATCH);
                request.AddQueryParameter("api-version", "1.6");

                request.AddHeader("Authorization", "Bearer " + token);
                request.AddHeader("Content-Type", "application/json");

                var body = JsonConvert.SerializeObject(updateReq);
                request.AddParameter("application/json", body, ParameterType.RequestBody);

                await client.ExecuteTaskAsync(request);

            }
            catch (Exception ex)
            {
                Log.Error("Update Azure AD Application {@error}", ex.Message);
                throw ex;
            }
        }

        private static async Task<ServicePrincipalResponse> CreateServicePrincipalIfNotExists(IApplication app, string password, ActiveDirectoryClient adClient)
        {
            try
            {
                var isNewSp = false;
                var spCreated = default(IServicePrincipal);

                // First check if the service principal exists, already
                var appIdToFilter = app.AppId;
                var spFilter = adClient.ServicePrincipals.Where(sp => sp.AppId == appIdToFilter);
                var foundSp = await spFilter.ExecuteAsync();
                if (foundSp.CurrentPage.Count == 0)
                {
                    spCreated = new ServicePrincipal
                    {
                        AccountEnabled = true,
                        AppId = app.AppId,
                        DisplayName = app.DisplayName,
                    };
                    spCreated.ServicePrincipalNames.Add(app.AppId);
                    spCreated.ServicePrincipalNames.Add(app.IdentifierUris.First());
                    spCreated.PasswordCredentials.Add(new PasswordCredential
                    {
                        StartDate = DateTime.UtcNow,
                        EndDate = DateTime.UtcNow.AddYears(1),
                        Value = password,
                        KeyId = Guid.NewGuid()
                    });

                    // Submit the creation request and return the newly created Service Principal Object
                    await adClient.ServicePrincipals.AddServicePrincipalAsync(spCreated);

                    isNewSp = true;
                }
                else
                {
                    spCreated = foundSp.CurrentPage.First();
                    spCreated.PasswordCredentials.Add(
                                            new PasswordCredential
                                            {
                                                StartDate = DateTime.UtcNow,
                                                EndDate = DateTime.UtcNow.AddYears(1),
                                                Value = password,
                                                KeyId = Guid.NewGuid()
                                            }
                                        );
                    await spCreated.UpdateAsync();
                }
                return new ServicePrincipalResponse { IsNewPrincipal = isNewSp, Principal = spCreated };
            }
            catch (Exception ex)
            {
                Log.Error("Update Azure AD Application {@error}", ex.Message);
                throw ex;
            }
        }

        private static string CreateRandomClientSecretKey()
        {
            using (RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider())
            {
                // Buffer storage.
                byte[] data = new byte[32];

                rng.GetBytes(data);
                var key = Convert.ToBase64String(data);
                return key;
            }
        }

        public static RequiredResourceAccess GetActiveDirectoryPermissions()
        {
            RequiredResourceAccess permissions = new RequiredResourceAccess()
            {
                // id for Azure Active Directory
                ResourceAppId = "00000002-0000-0000-c000-000000000000"
            };

            ResourceAccess ra2 = new ResourceAccess()
            {
                // id for Permission AAD Sign in and read user profile
                Id = new Guid("311a71cc-e848-46a1-bdf8-97ff7156d8e6"),
                Type = "Scope"
            };

            permissions.ResourceAccess.Add(ra2);
            return permissions;
        }
    }
}
