using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using MS.IoT.DeviceManagementPortal.Web;

namespace Microsoft.AspNetCore.Authentication
{
    public static class AzureAdAuthenticationBuilderExtensions
    {        
        public static AuthenticationBuilder AddAzureAd(this AuthenticationBuilder builder)
            => builder.AddAzureAd(_ => { });

        public static AuthenticationBuilder AddAzureAd(this AuthenticationBuilder builder, Action<AzureAdOptions> configureOptions)
        {
            builder.Services.Configure(configureOptions);
            builder.Services.AddSingleton<IConfigureOptions<OpenIdConnectOptions>, ConfigureAzureOptions>();
            builder.Services.AddSingleton<IConfigureOptions<CookieAuthenticationOptions>, ConfigureCookieOptions>();
            builder.AddOpenIdConnect();
            return builder;
        }

        public static AuthenticationBuilder AddAdal(this AuthenticationBuilder builder)
        {
            builder.Services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            builder.Services.AddSession();
            builder.Services.AddScoped(sp =>
            {
                var context = sp.GetService<IHttpContextAccessor>();
                var options = sp.GetService<IOptions<AzureAdOptions>>();
                var user = context.HttpContext.User;

                // Only for authenticated users
                if (!user.Identity.IsAuthenticated)
                    return null;

                var oid = user.FindFirst("oid").Value;
                var tid = user.FindFirst("tid").Value;

                return new AuthenticationContext($"{options.Value.Instance}{tid}", new ADALSessionCache(oid, context));
            });

            return builder;
        }

        private class ConfigureCookieOptions : IConfigureNamedOptions<CookieAuthenticationOptions>
        {
            private readonly IOptions<AzureAdOptions> azureOptions;
            private readonly IHttpContextAccessor contextAccessor;

            public ConfigureCookieOptions(IOptions<AzureAdOptions> azureOptions, IHttpContextAccessor contextAccessor)
            {
                this.azureOptions = azureOptions;
                this.contextAccessor = contextAccessor;
            }
            public void Configure(string name, CookieAuthenticationOptions options)
            {
                options.Events = new CookieAuthenticationEvents
                {
                    OnValidatePrincipal = context =>
                    {
                        var userId = context.Principal.FindFirst("oid").Value;
                        
                        // Check if exists in ADAL cache and reject if not. This happens if the cookie is alive and the server bounced
                        var cache = new ADALSessionCache(userId, contextAccessor);
                        if (cache.Count == 0)
                        {
                            context.RejectPrincipal();
                        }
                        return Task.CompletedTask; ;
                    }
                };
            }

            public void Configure(CookieAuthenticationOptions options)
            {
                Configure(Options.DefaultName, options);
            }
        }

        private class ConfigureAzureOptions: IConfigureNamedOptions<OpenIdConnectOptions>
        {
            private readonly AzureAdOptions _azureOptions;
            private readonly IHttpContextAccessor httpContextAccessor;
            private readonly string graphAppUri;

            public ConfigureAzureOptions(IOptions<AzureAdOptions> azureOptions, 
                                         IHttpContextAccessor httpContextAccessor, 
                                         IOptions<ResourceIds> resourceIds)
            {
                _azureOptions = azureOptions.Value;
                this.httpContextAccessor = httpContextAccessor;
                graphAppUri = resourceIds.Value.GraphId;
            }

            public void Configure(string name, OpenIdConnectOptions options)
            {
                options.ClientId = _azureOptions.ClientId;
                options.Authority = $"{_azureOptions.Instance}common";
                options.UseTokenLifetime = true;
                options.CallbackPath = _azureOptions.CallbackPath;
                options.RequireHttpsMetadata = true;
                options.ResponseType = OpenIdConnectResponseType.CodeIdToken;
                options.TokenValidationParameters.RoleClaimType = "roles";
                options.TokenValidationParameters.NameClaimType = "name";
                options.Scope.Add("offline_access");

                options.TokenValidationParameters = new TokenValidationParameters
                {
                    // Instead of using the default validation (validating against a single issuer value, as we do in line of business apps),
                    // we inject our own multitenant validation logic
                    ValidateIssuer = false,

                    // If the app is meant to be accessed by entire organizations, add your issuer validation logic here.
                    //IssuerValidator = (issuer, securityToken, validationParameters) => {
                    //    if (myIssuerValidationLogic(issuer)) return issuer;
                    //}
                };

                options.Events = new OpenIdConnectEvents
                {
                    OnTicketReceived = context =>
                    {
                        // If your authentication logic is based on users then add your logic here
                        return Task.CompletedTask;
                    },
                    OnAuthenticationFailed = context =>
                    {
                        context.Response.Redirect("/Home/Error");
                        context.HandleResponse(); // Suppress the exception
                        return Task.CompletedTask;
                    },
                    // If your application needs to do authenticate single users, add your user validation below.
                    //OnTokenValidated = context =>
                    //{
                    //    return myUserValidationLogic(context.Ticket.Principal);
                    //}
                    
                    OnAuthorizationCodeReceived = async context =>
                    {
                        var userId = context.Principal.FindFirst("oid").Value;
                        var tenantId = context.Principal.FindFirst("tid").Value;

                        // Get the tenant Id
                        var adal = new AuthenticationContext($"{_azureOptions.Instance}{tenantId}", new ADALSessionCache(userId, httpContextAccessor));

                        var redirect = $"{context.Request.Scheme}://{context.Request.Host}{context.Request.Path}";
                        // Store in cache for later redemption

                        var res = await adal.AcquireTokenByAuthorizationCodeAsync(context.ProtocolMessage.Code, new Uri(redirect), new ClientCredential(_azureOptions.ClientId, _azureOptions.ClientSecret), graphAppUri);

                        context.HandleCodeRedemption(res.AccessToken, res.IdToken);
                    }
                };
            }

            public void Configure(OpenIdConnectOptions options)
            {
                Configure(Options.DefaultName, options);
            }
        }
    }
}
