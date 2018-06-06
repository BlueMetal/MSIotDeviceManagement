using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SpaServices.Webpack;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using MS.IoT.DataPacketDesigner.Web.Helpers;
using MS.IoT.DataPacketDesigner.Web.Models;
using MS.IoT.Domain.Interface;
using MS.IoT.Domain.Model;
using MS.IoT.Repositories;

namespace MS.IoT.DataPacketDesigner.Web
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
                    .AddAzureAdBearer(options => Configuration.Bind("AzureAd", options));

            services.AddMvc();
            services.AddOptions();
            services.AddSession();

            services.Configure<IoTHubOptions>(Configuration.GetSection("IotHub"));
            services.Configure<BlobOptions>(Configuration.GetSection("Blob"));
            
            // Per Request services
            services.AddScoped<IUserProfileService, UserProfileService>();

            // Named options for the cosmos db. They're currently the same
            services.Configure<CosmosDbOptions>(typeof(Template).FullName, Configuration.GetSection("CosmosDb"));
            services.Configure<CosmosDbOptions>(typeof(Template).FullName, opt => opt.Collection = opt.Collections.Templates);
            services.Configure<CosmosDbOptions>(typeof(Category).FullName, Configuration.GetSection("CosmosDb"));
            services.Configure<CosmosDbOptions>(typeof(Category).FullName, opt => opt.Collection = opt.Collections.Templates);

            // This one is used by the blob storage controller to return settings
            services.Configure<CosmosDbOptions>(Configuration.GetSection("CosmosDb"));

            services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();


            services.AddScoped(typeof(ICosmosDBRepository<>), typeof(CosmosDBRepository<>));
            services.AddSingleton<IBlobStorageRepository, BlobStorageRepository>(sp => new BlobStorageRepository(Configuration["Blob:ConnectionString"]));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
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

            app.UseAuthentication();
            app.UseSession(new SessionOptions()
            {
                IdleTimeout = TimeSpan.FromHours(1)
            });

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");

                routes.MapSpaFallbackRoute(
                    name: "spa-fallback",
                    defaults: new { controller = "Home", action = "Index" });
            });      
        }
    }
}
