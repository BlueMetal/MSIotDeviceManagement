using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using MS.IoT.Domain.Interface;
using System.Threading.Tasks;
using MS.IoT.Domain.Model;
using System.Collections.Generic;
using System.Security.Cryptography;

namespace MS.IoT.Repositories.Tests
{
    [TestClass]
    public class ServicePrincipalRepositoryTests
    {
        public static readonly string tenantId = "72f43d57-b980-4152-b703-e2d8666a3ea9";
        public static readonly string clientId = "fd17f56b-cdd7-467f-ad3a-6b8b7bc7bc72";
        public static readonly string clientSecret = "ck30+q2nXvjfvAct8XC0ivWuZ3i6j/+n1SILJ4uGpGM=";
        public static string graphAuthToken;
        public static readonly string graphUrl = "https://graph.windows.net";

        [TestInitialize]
        public async Task SetupAsync()
        {
            string authContextURL = "https://login.microsoftonline.com/" + tenantId;
            var authenticationContext = new AuthenticationContext(authContextURL);
            var credential = new ClientCredential(clientId, clientSecret);
            var result = await authenticationContext.AcquireTokenAsync(graphUrl, credential);
            graphAuthToken = result.AccessToken;
        }

        [TestMethod]
        [ExpectedException(typeof(System.Data.Services.Client.DataServiceQueryException))]
        public async Task create_azureAD_app_service_principal_Exception()
        {
            var appUri = String.Format("https://{0}/{1}", tenantId, "unittestmsiot");
            ServicePrincipalRepository repo = new ServicePrincipalRepository(graphUrl);
            var app = await repo.CreateAppAndServicePrincipal("unittestmsiot", appUri, "msiot123",
                    tenantId, graphAuthToken+"sda");         
        }

        [TestMethod]
        [Ignore]
        public async Task create_azureAD_app_service_principal()
        {
            var appUri = String.Format("https://{0}/{1}", tenantId, "unittestmsiot");
            ServicePrincipalRepository repo = new ServicePrincipalRepository(graphUrl);
            var app = await repo.CreateAppAndServicePrincipal("unittestmsiot1", appUri, "msiot123",
                    tenantId, graphAuthToken);

            Assert.AreEqual("unittestmsiot1", app.App.DisplayName);        
        }

        [TestMethod]
        [Ignore]
        public async Task update_azureAD_Application()
        {
            var appUri = String.Format("https://{0}/{1}", tenantId, "unittestmsiot");
            ServicePrincipalRepository repo = new ServicePrincipalRepository(graphUrl);
            var app = await repo.CreateAppAndServicePrincipal("unittestmsiot", appUri, "msiot123",
                    tenantId, graphAuthToken);

            Assert.AreEqual("unittestmsiot", app.App.DisplayName);

            var updateModel = new UpdateApplicationRequest()
            {
                Homepage = "https://localhostunitest",
                ReplyUrls = new List<string>() {
                "https://localhostunitest"},
            };
            await repo.UpdateAzureADApplication(app.App.ObjectId, updateModel, 
                tenantId, graphAuthToken);

            app = await repo.CreateAppAndServicePrincipal("unittestmsiot", appUri, "msiot123",
                    tenantId, graphAuthToken);

            Assert.AreEqual(updateModel.Homepage, app.App.Homepage);
            Assert.IsTrue(app.App.ReplyUrls.Contains("https://localhostunitest"));
        }

        [TestMethod]
        [Ignore]
        public async Task update_azureAD_Application_PasswordCreds()
        {
            var appUri = String.Format("https://{0}/{1}", tenantId, "unittestmsiot");
            ServicePrincipalRepository repo = new ServicePrincipalRepository(graphUrl);
            var app = await repo.CreateAppAndServicePrincipal("unittestmsiot", appUri, "msiot123",
                    tenantId, graphAuthToken);

            Assert.AreEqual("unittestmsiot", app.App.DisplayName);

            var updateModel = new UpdateApplicationRequest()
            {
                Homepage = "https://localhostunitest",
                ReplyUrls = new List<string>() {
                "https://localhostunitest"},
            };
            await repo.UpdateAzureADApplication(app.App.ObjectId, updateModel,
                tenantId, graphAuthToken);

            app = await repo.CreateAppAndServicePrincipal("unittestmsiot", appUri, "msiot123",
                    tenantId, graphAuthToken);

            Assert.AreEqual(updateModel.Homepage, app.App.Homepage);
            Assert.IsTrue(app.App.ReplyUrls.Contains("https://localhostunitest"));

            // now update the password credentials object
            UpdateApplicationPasswordCredentials updateAppPasswordCreds =
                        new UpdateApplicationPasswordCredentials()
                        {
                            StartDate = DateTime.UtcNow,
                            EndDate = DateTime.UtcNow.AddYears(1),
                            Value = CreateRandomClientSecretKey(),
                            KeyId = Guid.NewGuid().ToString()
                        };
            var passwordList = new List<UpdateApplicationPasswordCredentials>();
            passwordList.Add(updateAppPasswordCreds);

            var updateAppPasswordReq = new UpdateApplicationPasswordCredsRequest()
            {
                UpdateApplicationPasswordCreds = passwordList
            };

            await repo.UpdateAzureADApplicationPasswordCredentials(app.App.ObjectId, updateAppPasswordReq,
                tenantId, graphAuthToken);

            app = await repo.CreateAppAndServicePrincipal("unittestmsiot", appUri, "msiot123",
                    tenantId, graphAuthToken);


            Assert.AreEqual(updateAppPasswordCreds.StartDate,app.App.PasswordCredentials[0].StartDate);
            Assert.AreEqual(updateAppPasswordCreds.EndDate, app.App.PasswordCredentials[0].EndDate);
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
}

