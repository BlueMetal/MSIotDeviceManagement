using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.ActiveDirectory.GraphClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using MS.IoT.Domain.Interface;
using MS.IoT.Repositories;

namespace MS.IoT.MarketingPortal.Web
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

            services.AddAuthentication(sharedOptions =>
            {
                sharedOptions.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                sharedOptions.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
            })
            .AddAzureAd(options => Configuration.Bind("AzureAd", options))
            .AddCookie()
            .AddAdal();
                    

            services.AddMvc();

            // Options
            services.Configure<ResourceIds>(Configuration.GetSection("ResourceIds"));
            services.Configure<UrlOptions>(Configuration.GetSection("Urls"));
            services.Configure<UrlOptions>(opt =>
            {
                var sasToken = Configuration["SASToken"];
                opt.DataPacketDesignerPackage += sasToken;
                opt.DeviceManagementPortalPackage += sasToken;
                opt.WatchVideo += sasToken;
            });
            services.Configure<ArmOptions>(Configuration.GetSection("ResourceManager"));
            services.Configure<CosmosDbOptions>(Configuration.GetSection("CosmosDb"));

            services.AddScoped<ITokenServices, TokenServices>();
            services.AddScoped(sp =>
            {
                var tokenService = sp.GetService<ITokenServices>();
                if (tokenService.TenantId != null)
                {
                    return new ActiveDirectoryClient(new Uri(tokenService.DirectoryInstance), () => tokenService.GetGraphTokenAsync());
                }

                return null;
            });

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddScoped<IResourceManagerRepository, ResourceManagerRepository>();
            services.AddScoped<IServicePrincipalRepository, ServicePrincipalRepository>();
            services.AddScoped<IArmClientFactory, ArmClientFactory>(sp =>
            {
                var tokenService = sp.GetService<ITokenServices>();
                var opts = sp.GetService<IOptionsSnapshot<ArmOptions>>();
                if (tokenService.TenantId != null)
                {
                    return new ArmClientFactory(() => tokenService.GetAzureManagementTokenAsync(), opts.Value.EndpointAddress);
                }

                return null;
            });

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();

            app.UseSession(new SessionOptions()
            {
                IdleTimeout = TimeSpan.FromHours(1)
            });

            app.UseAuthentication();


            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
