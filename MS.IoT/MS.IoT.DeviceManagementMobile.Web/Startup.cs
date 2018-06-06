using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.IdentityModel.Tokens.Jwt;
using MS.IoT.Domain.Interface;
using MS.IoT.Repositories;
using Swashbuckle.AspNetCore.Swagger;

namespace MS.IoT.DeviceManagementMobile.Web
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
                sharedOptions.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddAzureAdB2CBearer(options => Configuration.Bind("AzureAdB2C", options));


            services.AddMvc();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info { Title = "MS.IoT.DeviceManagementMobile.Web", Version = "v1" });
            });


            services.AddScoped<INotificationHubRepository, NotificationHubRepository>(sp => new NotificationHubRepository(Configuration["NotificationHub:ConnectionString"]));
            services.AddScoped<IDeviceTwinRepository, DeviceTwinRepository>(sp => new DeviceTwinRepository(Configuration["IotHub:ConnectionString"]));
            services.AddScoped<IUserDeviceTwinRepository, UserDeviceTwinRepository>(sp => new UserDeviceTwinRepository(Configuration["IotHub:ConnectionString"]));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseAuthentication();

            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger();

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.), specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "MS.IoT.DeviceManagementMobile.Web V1");
            });

            app.UseMvc();
        }
    }
}
