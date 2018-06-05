using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;
using System.Threading.Tasks;
using Microsoft.Azure.ActiveDirectory.GraphClient;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OpenIdConnect;
using MS.IoT.DeviceManagementPortal.Web.Helpers;

namespace MS.IoT.MarketingPortal.Web.Controllers
{
    [Authorize]
    public class UserProfileController : Controller
    {
        // GET: UserProfile
        public async Task<ActionResult> Index()
        {
            string tenantID = ClaimsPrincipal.Current.FindFirst("http://schemas.microsoft.com/identity/claims/tenantid").Value;
            string userObjectID = ClaimsPrincipal.Current.FindFirst("http://schemas.microsoft.com/identity/claims/objectidentifier").Value;
            try
            {
                Uri servicePointUri = new Uri(AuthConfig.ConfigurationItems.GraphAppUri);
                Uri serviceRoot = new Uri(servicePointUri, tenantID);
                var authcode = AuthConfig.SessionItems.AuthCode;
                if (authcode == null)
                {
                    RefreshSession();
                }
                var userObjectCode = AuthConfig.SessionItems.UserObjectId;

                ActiveDirectoryClient activeDirectoryClient = new ActiveDirectoryClient(serviceRoot,
                      async () => await GetTokenForApplication1(authcode));

                // use the token for querying the graph to get the user details

                var result = await activeDirectoryClient.Users
                    .Where(u => u.ObjectId.Equals(userObjectID))
                    .ExecuteAsync();
                IUser user = result.CurrentPage.ToList().First();

                return View(user);
            }
            catch (AdalException ae)
            {
                // Return to error page.
                return View("Error");
            }
            // if the above failed, the user needs to explicitly re-authenticate for the app to obtain the required token
            catch (Exception ex)
            {
                return View("Relogin");
            }
        }

        public void RefreshSession()
        {
            HttpContext.GetOwinContext().Authentication.Challenge(
                new AuthenticationProperties { RedirectUri = "/UserProfile" },
                OpenIdConnectAuthenticationDefaults.AuthenticationType);
        }


        public async Task<string> GetTokenForApplication1(string code)
        {
            string signedInUserID = ClaimsPrincipal.Current.FindFirst(ClaimTypes.NameIdentifier).Value;
            string tenantID = ClaimsPrincipal.Current.FindFirst("http://schemas.microsoft.com/identity/claims/tenantid").Value;
            var authContext = new AuthenticationContext(AuthConfig.ConfigurationItems.AADInstance + tenantID);
            var authResult = await authContext.AcquireTokenByAuthorizationCodeAsync(
                                    code,
                                    new System.Uri(AuthConfig.ConfigurationItems.RedirectUri),
                                    new ClientCredential(AuthConfig.ConfigurationItems.ClientId, AuthConfig.ConfigurationItems.ClientSecret),
                                    AuthConfig.ConfigurationItems.GraphAppUri
                                );
            return authResult.AccessToken;
        }
    }
}
