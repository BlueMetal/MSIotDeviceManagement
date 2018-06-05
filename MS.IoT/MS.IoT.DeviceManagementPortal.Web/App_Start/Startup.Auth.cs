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
using MS.IoT.DeviceManagementPortal.Web.Helpers;
using MS.IoT.Common;
using System.IdentityModel.Tokens;
using Microsoft.Owin.Security.ActiveDirectory;

namespace MS.IoT.DeviceManagementPortal.Web
{
    public partial class Startup
    {
        public void ConfigureAuth(IAppBuilder app)
        {
            app.SetDefaultSignInAsAuthenticationType(CookieAuthenticationDefaults.AuthenticationType);

            app.UseCookieAuthentication(new CookieAuthenticationOptions());

            app.UseWindowsAzureActiveDirectoryBearerAuthentication(
                new WindowsAzureActiveDirectoryBearerAuthenticationOptions
                {
                    Tenant = AuthConfig.ConfigurationItems.TenantDomain,
                    AuthenticationType = "OAuth2Bearer",
                    TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidAudience = AuthConfig.ConfigurationItems.Audience
                    },
                });

            app.UseOpenIdConnectAuthentication(
                new OpenIdConnectAuthenticationOptions
                {
                    ClientId = AuthConfig.ConfigurationItems.ClientId,
                    Authority = AuthConfig.ConfigurationItems.Authority,
                    RedirectUri = AuthConfig.ConfigurationItems.RedirectUri,
                    PostLogoutRedirectUri = AuthConfig.ConfigurationItems.RedirectUri,
                    TokenValidationParameters = new System.IdentityModel.Tokens.TokenValidationParameters
                    {
                        // instead of using the default validation (validating against a single issuer value, as we do in line of business apps), 
                        // we inject our own multitenant validation logic
                        ValidateIssuer = false,
                    },
                    Notifications = new OpenIdConnectAuthenticationNotifications()
                    {
                        SecurityTokenValidated = (context) =>
                        {
                            return Task.FromResult(0);
                        },
                        AuthorizationCodeReceived = async (context) =>
                        {
                            var code = context.Code;
                            AuthConfig.SessionItems.AuthCode = context.Code;
                            AuthConfig.SessionItems.AuthCodeLastTokenRequestUrl = new Uri(HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Path));
                            AuthConfig.SessionItems.UserObjectId = context.AuthenticationTicket.Identity.FindFirst("http://schemas.microsoft.com/identity/claims/objectidentifier").Value;
                            AuthConfig.SessionItems.GraphTargetTenant = context.AuthenticationTicket.Identity.FindFirst("http://schemas.microsoft.com/identity/claims/tenantid").Value;
                           // AuthConfig.SessionItems.UserUPN = context.AuthenticationTicket.Identity.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/upn").Value;

                            Log.Information("Startup Auth : Authenticated {authcode}", AuthConfig.SessionItems.AuthCode);
                            try
                            {
                                var authenticationContext = new Microsoft.IdentityModel.Clients.ActiveDirectory.AuthenticationContext(AuthConfig.ConfigurationItems.AADInstance + AuthConfig.SessionItems.GraphTargetTenant);
                                var credential = new ClientCredential(AuthConfig.ConfigurationItems.ClientId,
                                    AuthConfig.ConfigurationItems.ClientSecret);

                                // get authtoken
                                //var resultAuth = await authenticationContext.AcquireTokenByAuthorizationCodeAsync(
                                //    AuthConfig.SessionItems.AuthCode, new Uri(AuthConfig.ConfigurationItems.RedirectUri),
                                //    credential, "e8b29c1b-5f3f-4fe7-93da-e147ae3ab60d");

                              
                                //// get management Token
                                //var resultManagement = await authenticationContext.AcquireTokenByAuthorizationCodeAsync(
                                //    AuthConfig.SessionItems.AuthCode, new Uri(AuthConfig.ConfigurationItems.RedirectUri),
                                //    credential, AuthConfig.ConfigurationItems.ManagementAppUri);

                                //AuthConfig.SessionItems.ManagementAuthToken = resultManagement.AccessToken;
                                //Log.Information("Startup Auth : Get Management token {token}", AuthConfig.SessionItems.ManagementAuthToken);

                                //// get graph token
                                //var resultGraph = await authenticationContext.AcquireTokenByAuthorizationCodeAsync(
                                //    AuthConfig.SessionItems.AuthCode, new Uri(AuthConfig.ConfigurationItems.RedirectUri),
                                //    credential, AuthConfig.ConfigurationItems.GraphAppUri);
                                //AuthConfig.SessionItems.GraphAuthToken = resultGraph.AccessToken;
                                //Log.Information("Startup Auth : Get Graph token {token}", AuthConfig.SessionItems.GraphAuthToken);
                            }
                            catch (Exception ex)
                            {
                                Log.Error("Startup Auth : Exception {message}", ex.Message);
                            }
                        }
                        //AuthenticationFailed = (context) =>
                        //{
                        //    context.OwinContext.Response.Redirect("/Home/Error");
                        //    context.HandleResponse(); // Suppress the exception
                        //    return Task.FromResult(0);
                        //}                       
                    }
                });
         }
    }
}
