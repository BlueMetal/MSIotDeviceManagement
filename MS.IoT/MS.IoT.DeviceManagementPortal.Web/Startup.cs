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
using Microsoft.AspNetCore.SpaServices.Webpack;
using Microsoft.Azure.ActiveDirectory.GraphClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MS.IoT.DeviceManagementPortal.Web.Helpers;
using MS.IoT.Domain.Interface;
using MS.IoT.Domain.Model;
using MS.IoT.Repositories;
using MS.IoT.Repositories.Services;
using Quartz;
using Quartz.Spi;

namespace MS.IoT.DeviceManagementPortal.Web
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
            services.AddOptions();
            services.AddSession();

            // Options
            services.Configure<ResourceIds>(Configuration.GetSection("ResourceIds"));
            services.Configure<ArmOptions>(Configuration.GetSection("ResourceManager"));
            services.Configure<IoTHubOptions>(Configuration.GetSection("IotHub"));

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
            services.AddScoped<IArmClientFactory, ArmClientFactory>(sp =>
            {
                var tokenService = sp.GetService<ITokenServices>();
                if (tokenService.TenantId != null)
                {
                    return new ArmClientFactory(() => tokenService.GetAzureManagementTokenAsync(), Configuration["ResourceManager:EndpointAddress"]);
                }

                return null;
            });

            // Named options for the cosmos db. They're currently the same
            services.Configure<CosmosDbOptions>(typeof(CustomGroupModel).FullName, Configuration.GetSection("CosmosDb"));
            services.Configure<CosmosDbOptions>(typeof(CustomGroupModel).FullName, opt => opt.Collection = opt.Collections.Groups);

            services.AddScoped(typeof(ICosmosDBRepository<>), typeof(CosmosDBRepository<>));
            services.AddScoped<IUserProfileService, UserProfileService>();
            services.AddScoped<INotificationHubRepository, NotificationHubRepository>(sp => new NotificationHubRepository(Configuration["NotificationHub:ConnectionString"]));

            services.AddSingleton<IDeviceDBService, DeviceDBService>(sp => new DeviceDBService(Configuration["IotHub:ConnectionString"]));

            //Quartz
            services.AddScoped<QuartzRefreshDBJob>();
            services.AddQuartz();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IServiceProvider container)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                /* app.UseWebpackDevMiddleware(new WebpackDevMiddlewareOptions
                {
                    HotModuleReplacement = true
                }); TO REACTIVATE */
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

                routes.MapSpaFallbackRoute(
                    name: "spa-fallback",
                    defaults: new { controller = "Home", action = "Index" });
            });

            app.UseQuartz(Configuration["IotHub:CacheRefreshMinutes"]);
        }
    }
}
