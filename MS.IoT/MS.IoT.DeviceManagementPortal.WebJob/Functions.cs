using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using System.Net.Http;
using System.Net.Http.Headers;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using System.Configuration;

namespace MS.IoT.DeviceManagementPortal.WebJob
{
    public class Functions
    {
        private static readonly HttpClient _Client = new HttpClient();
        private static readonly string tenantId = ConfigurationManager.AppSettings["ida:TenantId"];
        private static readonly string instance = ConfigurationManager.AppSettings["ida:AADInstance"];
        private static readonly string authority = instance + tenantId;
        private static readonly string clientId = ConfigurationManager.AppSettings["ida:ClientId"];
        private static readonly string clientSecret = ConfigurationManager.AppSettings["ida:ClientSecret"];
        private static readonly string resource = ConfigurationManager.AppSettings["ida:Resource"];

        public async static Task RefreshDB([TimerTrigger("0 */2 * * * *", RunOnStartup = true)] TimerInfo timer, TraceWriter log)
        {
            log.Info("Entering RefreshDB...");
            string apiCall = string.Format(ConfigurationManager.AppSettings["API_REFRESH_CALL"], Environment.GetEnvironmentVariable("WEBSITE_HOSTNAME"));
            try
            {
                AuthenticationResult auth = await GetAccessToken();
                using (HttpRequestMessage message = new HttpRequestMessage(HttpMethod.Get, apiCall))
                {
                    message.Headers.Authorization = new AuthenticationHeaderValue(auth.AccessTokenType, auth.AccessToken);

                    HttpResponseMessage response = await _Client.SendAsync(message);
                    response.EnsureSuccessStatusCode();
                    string responseBody = await response.Content.ReadAsStringAsync();
                    if (responseBody != "true")
                        log.Error("Error while refreshing the cache.");
                    else
                        log.Info("Cache refreshed.");
                }
            }
            catch(Exception e)
            {
                log.Error(e.Message);
            }
            log.Info("Leaving RefreshDB...");
        }

        private async static Task<AuthenticationResult> GetAccessToken()
        {
            var authenticationContext = new AuthenticationContext(authority);
            var credential = new ClientCredential(clientId, clientSecret);
            var result = await authenticationContext.AcquireTokenAsync(resource, credential);

            if (result == null)
            {
                throw new InvalidOperationException("Failed to obtain the JWT token");
            }

            return result;
        }

    }
}
