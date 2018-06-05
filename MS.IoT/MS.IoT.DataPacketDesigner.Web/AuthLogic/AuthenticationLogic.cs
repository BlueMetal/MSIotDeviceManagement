using Microsoft.IdentityModel.Clients.ActiveDirectory;
using System;
using System.Threading.Tasks;

namespace MS.IoT.DataPacketDesigner.Web
{
    /// <summary>
    /// Class helper AuthenticationLogic to retrieve a token and build the proper connection urls
    /// </summary>
    public static class AuthenticationLogic
    {
        public static async Task<AuthenticationResult> RedeemTokenAsync(string targetResourceUri, Uri originalReplyUrl)
        {

            var code = AuthenticationConfig.SessionItems.AuthCode;
            var userObjectId = AuthenticationConfig.SessionItems.UserObjectId;

            var authContext = new AuthenticationContext
                                    (
                                        AuthenticationConfig.ConfigurationItems.Authority,
                                        new AdalSimpleTokenCache(userObjectId)
                                    );
            var authResult = await authContext.AcquireTokenByAuthorizationCodeAsync(
                                    code,
                                    originalReplyUrl,
                                    new ClientCredential(AuthenticationConfig.ConfigurationItems.ClientId, AuthenticationConfig.ConfigurationItems.AppKey),
                                    targetResourceUri
                                );
            return authResult;

        }

        public static async Task<string> GetTokensForNeededServices()
        {
            // Try get the token for the service management APIs
            //var token = await AuthenticationLogic.RedeemTokenAsync
            //            (
            //                AuthenticationConfig.ConfiguratinItems.ManagementAppUri,
            //                AuthenticationConfig.SessionItems.AuthCodeLastTokenRequestUrl
            //            );
            //AuthenticationConfig.SessionItems.ManagementAuthToken = token.AccessToken;

            // Then try getting the token for Graph API calls
            var graphToken = await AuthenticationLogic.RedeemTokenAsync
                                    (
                                        AuthenticationConfig.ConfigurationItems.GraphAppUri,
                                        AuthenticationConfig.SessionItems.AuthCodeLastTokenRequestUrl
                                    );
            AuthenticationConfig.SessionItems.GraphAuthToken = graphToken.AccessToken;
            AuthenticationConfig.SessionItems.GraphTargetTenant = graphToken.TenantId;

            return AuthenticationConfig.SessionItems.GraphAuthToken;
        }

        public static async Task<string> ConstructConsentUrlAsync(string tenantDomainOrId, string targetResourceUri, string redirectUrl, bool isAdmin = false)
        {
            var authorizationUrl = string.Empty;
            if (!string.IsNullOrEmpty(targetResourceUri))
            {
                authorizationUrl = string.Format(
                    "https://login.windows.net/{0}/oauth2/authorize?" +
                        "api-version=1.0&" +
                        "response_type=code&client_id={1}&" +
                        "resource={2}&" +
                        "redirect_uri ={3}",
                    tenantDomainOrId,
                    AuthenticationConfig.ConfigurationItems.ClientId,
                    Uri.EscapeUriString(targetResourceUri),
                    redirectUrl
                );
            }
            else
            {
                authorizationUrl = string.Format(
                    "https://login.windows.net/{0}/oauth2/authorize?" +
                        "api-version=1.0&" +
                        "response_type=code&client_id={1}&" +
                        "redirect_uri ={2}",
                    tenantDomainOrId,
                    AuthenticationConfig.ConfigurationItems.ClientId,
                    redirectUrl
                );

            }

            if(isAdmin)
            {
                authorizationUrl += "&prompt=admin_consent";
            }

            return await Task.FromResult<string>(authorizationUrl);
        }
    }
}