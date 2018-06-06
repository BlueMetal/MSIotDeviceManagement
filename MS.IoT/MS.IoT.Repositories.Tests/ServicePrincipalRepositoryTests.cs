using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Data.Services.Client;
using System.Linq;
using System.Security.Cryptography;
using Microsoft.Azure.ActiveDirectory.GraphClient;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using MS.IoT.Domain.Model;
using MS.IoT.Repositories;
using Xunit;

public class ServicePrincipalRepositoryTests : IAsyncLifetime
{
    public static readonly string tenantId = "72f43d57-b980-4152-b703-e2d8666a3ea9";
    public static readonly string clientId = "fd17f56b-cdd7-467f-ad3a-6b8b7bc7bc72";
    public static readonly string clientSecret = "ck30+q2nXvjfvAct8XC0ivWuZ3i6j/+n1SILJ4uGpGM=";
    public static string graphAuthToken;
    public static readonly string graphUrl = "https://graph.windows.net";

    ActiveDirectoryClient activeDirectoryClient;
    ILogger<ServicePrincipalRepository> logger;

    public async Task InitializeAsync()
    {
        string authContextURL = "https://login.microsoftonline.com/" + tenantId;
        var authenticationContext = new AuthenticationContext(authContextURL);
        var credential = new ClientCredential(clientId, clientSecret);
        var result = await authenticationContext.AcquireTokenAsync(graphUrl, credential);
        graphAuthToken = result.AccessToken;

        activeDirectoryClient = new ActiveDirectoryClient(new Uri($"{authContextURL}{tenantId}"), () => Task.FromResult(graphAuthToken));
        logger = new LoggerFactory().CreateLogger<ServicePrincipalRepository>();
    }

    public Task DisposeAsync()
    {
        return Task.CompletedTask;
    }

    [Fact(Skip = "Skipped")]
    public async Task create_azureAD_app_service_principal()
    {
        var appUri = string.Format("https://{0}/{1}", tenantId, "unittestmsiot");
        var repo = new ServicePrincipalRepository(activeDirectoryClient, logger);
        var app = await repo.CreateAppAndServicePrincipal("unittestmsiot1",
                                                          appUri,
                                                          "msiot123",
                                                          tenantId);

        Assert.Equal("unittestmsiot1", app.App.DisplayName);
    }

    [Fact]
    public async Task create_azureAD_app_service_principal_Exception()
    {
        var appUri = string.Format("https://{0}/{1}", tenantId, "unittestmsiot");
        var repo = new ServicePrincipalRepository(activeDirectoryClient, logger);
        await Assert.ThrowsAsync<DataServiceQueryException>(async () => await repo.CreateAppAndServicePrincipal("unittestmsiot",
                                                                                                                appUri,
                                                                                                                "msiot123",
                                                                                                                tenantId+"sddra"));
    }

    [Fact(Skip = "Skipped")]
    public async Task update_azureAD_Application()
    {
        var appUri = string.Format("https://{0}/{1}", tenantId, "unittestmsiot");
        var repo = new ServicePrincipalRepository(activeDirectoryClient, logger);
        var app = await repo.CreateAppAndServicePrincipal("unittestmsiot",
                                                          appUri,
                                                          "msiot123",
                                                          tenantId);

        Assert.Equal("unittestmsiot", app.App.DisplayName);

        var updateModel = new UpdateApplicationRequest
        {
            Homepage = "https://localhostunitest",
            ReplyUrls = new List<string>
            {
                "https://localhostunitest"
            }
        };
        await repo.UpdateAzureADApplication(app.App.ObjectId,
                                            updateModel,
                                            tenantId);

        app = await repo.CreateAppAndServicePrincipal("unittestmsiot",
                                                      appUri,
                                                      "msiot123",
                                                      tenantId);

        Assert.Equal(updateModel.Homepage, app.App.Homepage);
        Assert.True(app.App.ReplyUrls.Contains("https://localhostunitest"));
    }

    [Fact(Skip = "Skipped")]
    public async Task update_azureAD_Application_PasswordCreds()
    {
        var appUri = string.Format("https://{0}/{1}", tenantId, "unittestmsiot");
        var repo = new ServicePrincipalRepository(activeDirectoryClient, logger);
        var app = await repo.CreateAppAndServicePrincipal("unittestmsiot",
                                                          appUri,
                                                          "msiot123",
                                                          tenantId);

        Assert.Equal("unittestmsiot", app.App.DisplayName);

        var updateModel = new UpdateApplicationRequest
        {
            Homepage = "https://localhostunitest",
            ReplyUrls = new List<string>
            {
                "https://localhostunitest"
            }
        };
        await repo.UpdateAzureADApplication(app.App.ObjectId,
                                            updateModel,
                                            tenantId);

        app = await repo.CreateAppAndServicePrincipal("unittestmsiot",
                                                      appUri,
                                                      "msiot123",
                                                      tenantId);

        Assert.Equal(updateModel.Homepage, app.App.Homepage);
        Assert.True(app.App.ReplyUrls.Contains("https://localhostunitest"));

        // now update the password credentials object
        UpdateApplicationPasswordCredentials updateAppPasswordCreds =
            new UpdateApplicationPasswordCredentials
            {
                StartDate = DateTime.UtcNow,
                EndDate = DateTime.UtcNow.AddYears(1),
                Value = CreateRandomClientSecretKey(),
                KeyId = Guid.NewGuid()
            };
        var passwordList = new List<UpdateApplicationPasswordCredentials>();
        passwordList.Add(updateAppPasswordCreds);

        var updateAppPasswordReq = new UpdateApplicationPasswordCredsRequest
        {
            UpdateApplicationPasswordCreds = passwordList
        };

        await repo.UpdateAzureADApplicationPasswordCredentials(app.App.ObjectId,
                                                               updateAppPasswordReq,
                                                               tenantId);

        app = await repo.CreateAppAndServicePrincipal("unittestmsiot",
                                                      appUri,
                                                      "msiot123",
                                                      tenantId);


        Assert.Equal(updateAppPasswordCreds.StartDate,
                     app.App.PasswordCredentials[0]
                        .StartDate);
        Assert.Equal(updateAppPasswordCreds.EndDate,
                     app.App.PasswordCredentials[0]
                        .EndDate);
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
}