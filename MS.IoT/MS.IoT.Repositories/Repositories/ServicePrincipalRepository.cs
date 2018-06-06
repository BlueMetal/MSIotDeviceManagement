using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Microsoft.Azure.ActiveDirectory.GraphClient;
using Microsoft.Extensions.Logging;
using MS.IoT.Domain.Interface;
using MS.IoT.Domain.Model;
using Newtonsoft.Json;

namespace MS.IoT.Repositories
{

    public class ServicePrincipalRepository : IServicePrincipalRepository
    {
        private readonly ActiveDirectoryClient adClient;
        private readonly ILogger<ServicePrincipalRepository> logger;

        public ServicePrincipalRepository(ActiveDirectoryClient activeDirectoryClient, ILogger<ServicePrincipalRepository> logger)
        {
            adClient = activeDirectoryClient;
            this.logger = logger;
        }

        public async Task<ServicePrincipalResponse> CreateAppAndServicePrincipal(string displayName, string appIdUri, string password, string tenantId)
        {
            try
            {
                // First create the app backing up the service principal          
                var appResponse = await CreateAzureADApplicationIfNotExists(displayName, appIdUri, tenantId);

                // Now we can create the service principal to be created in the AD
                var spResponse = await CreateServicePrincipalIfNotExists(appResponse.App, password);

                // Finally return the AppId
                return new ServicePrincipalResponse
                {
                    App = appResponse.App,
                    AppClientSecret = appResponse.AppClientSecret,
                    IsNewApp = appResponse.IsNewApp,
                    Principal = spResponse.Principal,
                    IsNewPrincipal = spResponse.IsNewPrincipal
                };
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Create Azure AD Application {error}", ex.Message);
                throw;
            }
        }

        private async Task<ServicePrincipalResponse> CreateAzureADApplicationIfNotExists(string displayName, string appIdUri, string tenantId)
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
                    var updateAppPasswordCreds = new UpdateApplicationPasswordCredentials()
                        {
                            StartDate = DateTime.UtcNow,
                            EndDate = DateTime.UtcNow.AddYears(2),
                            Value = CreateRandomClientSecretKey(),
                            KeyId = Guid.NewGuid()
                        };
                    var passwordList = new List<UpdateApplicationPasswordCredentials>();
                    passwordList.Add(updateAppPasswordCreds);

                    var updateAppPasswordReq = new UpdateApplicationPasswordCredsRequest()
                    {
                        UpdateApplicationPasswordCreds = passwordList
                    };

                    await UpdateAzureADApplicationPasswordCredentialsRest(appCreated.ObjectId, updateAppPasswordReq, tenantId);
                    spr.AppClientSecret = updateAppPasswordCreds.Value;
                }

                spr.App = appCreated;
                spr.IsNewApp = isNewApp;
                return spr;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Create Azure AD Application {error}", ex.Message);
                throw;
            }
        }

        public async Task UpdateAzureADApplication(string appObjectId, UpdateApplicationRequest updateReq, string tenantId)
        {
            try
            {

                var adApp = await adClient.Applications.GetByObjectId(appObjectId).ExecuteAsync();
                adApp.ReplyUrls.Clear();
                updateReq.ReplyUrls.ForEach(adApp.ReplyUrls.Add);

                adApp.Homepage = updateReq.Homepage;

                await adApp.UpdateAsync();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Update Azure AD Application {error}", ex.Message);
                throw;
            }
        }

        public async Task UpdateAzureADApplicationPasswordCredentials(string appObjectId, UpdateApplicationPasswordCredsRequest updateReq, string tenantId)
        {
            try
            {
                await UpdateAzureADApplicationPasswordCredentialsRest(appObjectId, updateReq, tenantId);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Update Azure AD Application {error}", ex.Message);
                throw;
            }
        }

        private async Task UpdateAzureADApplicationPasswordCredentialsRest(string appObjectId, UpdateApplicationPasswordCredsRequest updateReq, string tenantId)
        {
            try
            {
                var adApp = await adClient.Applications.GetByObjectId(appObjectId).ExecuteAsync();
                adApp.PasswordCredentials.Clear();
                foreach (var req in updateReq.UpdateApplicationPasswordCreds)
                {
                    adApp.PasswordCredentials.Add(new PasswordCredential
                    {
                        Value = req.Value,
                        KeyId = req.KeyId,
                        StartDate = req.StartDate,
                        EndDate = req.EndDate
                    });
                }

                await adApp.UpdateAsync();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Update Azure AD Application {error}", ex.Message);
                throw;
            }
        }

        private async Task<ServicePrincipalResponse> CreateServicePrincipalIfNotExists(IApplication app, string password)
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
                logger.LogError(ex, "Update Azure AD Application {@error}", ex.Message);
                throw;
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
