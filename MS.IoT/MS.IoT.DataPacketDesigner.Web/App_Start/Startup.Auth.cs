using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.IdentityModel.Claims;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OpenIdConnect;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Owin;
using MS.IoT.DataPacketDesigner.Web.Models;

namespace MS.IoT.DataPacketDesigner.Web
{
    public partial class Startup
    {
        public void ConfigureAuth(IAppBuilder app)
        {
           // ApplicationDbContext db = new ApplicationDbContext();

            app.SetDefaultSignInAsAuthenticationType(CookieAuthenticationDefaults.AuthenticationType);

            app.UseCookieAuthentication(new CookieAuthenticationOptions());

            app.UseOpenIdConnectAuthentication(
                new OpenIdConnectAuthenticationOptions
                {
                    ClientId = AuthenticationConfig.ConfigurationItems.ClientId,
                    Authority = AuthenticationConfig.ConfigurationItems.Authority,
                    RedirectUri = AuthenticationConfig.ConfigurationItems.PostLogoutRedirectUri,
                    PostLogoutRedirectUri = AuthenticationConfig.ConfigurationItems.PostLogoutRedirectUri,

                    Notifications = new OpenIdConnectAuthenticationNotifications()
                    {
                        // If there is a code in the OpenID Connect response, redeem it for an access token and refresh token, and store those away.
                        AuthorizationCodeReceived = (context) =>
                        {
                            var code = context.Code;
                            ClientCredential credential = new ClientCredential(AuthenticationConfig.ConfigurationItems.ClientId, AuthenticationConfig.ConfigurationItems.AppKey);
                            string signedInUserID = context.AuthenticationTicket.Identity.FindFirst(ClaimTypes.NameIdentifier).Value;
                            //AuthenticationContext authContext = new AuthenticationContext(AuthenticationConfig.ConfigurationItems.Authority, new ADALTokenCache(signedInUserID));
                            //AuthenticationResult result = authContext.AcquireTokenByAuthorizationCode(
                            //code, new Uri(HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Path)), credential, AuthenticationConfig.ConfigurationItems.GraphAppUri);

                            return Task.FromResult(0);
                        }
                    }
                });
        }
    }
}
