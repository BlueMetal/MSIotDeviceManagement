using Microsoft.Owin.Security.Jwt;
using Microsoft.Owin.Security.OAuth;
using MS.IoT.DeviceManagementMobile.Web.Helpers;
using Owin;
using System;
using System.Configuration;
using System.IdentityModel.Tokens;

namespace MS.IoT.DeviceManagementMobile.Web
{
    public partial class Startup
    {
        // These values are pulled from web.config
        public static string SignUpSignInPolicy = AuthConfig.ConfigurationItems.SignUpSignInPolicy;
        public static string DefaultPolicy = SignUpSignInPolicy;

        // For more information on configuring authentication, please visit https://go.microsoft.com/fwlink/?LinkId=301864
        public void ConfigureAuth(IAppBuilder app)
        {
            TokenValidationParameters tvps = new TokenValidationParameters
            {
                // Accept only those tokens where the audience of the token is equal to the client ID of this app
                ValidAudience = AuthConfig.ConfigurationItems.ClientId,
                AuthenticationType = Startup.DefaultPolicy
            };

            app.UseOAuthBearerAuthentication(new OAuthBearerAuthenticationOptions
            {
                // This SecurityTokenProvider fetches the Azure AD B2C metadata & signing keys from the OpenIDConnect metadata endpoint
                AccessTokenFormat = new JwtFormat(tvps, new OpenIdConnectCachingSecurityTokenProvider(
                    String.Format(AuthConfig.ConfigurationItems.AadInstance, AuthConfig.ConfigurationItems.Tenant, DefaultPolicy)))
            });

            //app.UseWindowsAzureActiveDirectoryBearerAuthentication(
            //    new WindowsAzureActiveDirectoryBearerAuthenticationOptions
            //    {
            //        Tenant = ConfigurationManager.AppSettings["ida:Tenant"],
            //        TokenValidationParameters = new TokenValidationParameters {
            //             ValidAudience = ConfigurationManager.AppSettings["ida:Audience"]
            //        },
            //    });
        }
    }
}
