using Microsoft.Azure.ActiveDirectory.GraphClient;
using Microsoft.Practices.Unity;
using Microsoft.Practices.Unity.Mvc;
using MS.IoT.DataPacketDesigner.Web.Helpers;
using MS.IoT.Domain.Interface;
using MS.IoT.Domain.Model;
using MS.IoT.Repositories;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web.Http;

namespace MS.IoT.DataPacketDesigner.Web
{
    /// <summary>
    /// Class to configure the API and dependency injections
    /// </summary>
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            IUnityContainer container = new UnityContainer();
            container.RegisterType<ICosmosDBRepository<Template>, CosmosDBRepository<Template>>(new ContainerControlledLifetimeManager(), new InjectionConstructor(
                AppConfig.ConfigurationItems.EndPoint,
                AppConfig.ConfigurationItems.AuthKey,
                AppConfig.ConfigurationItems.Database,
                AppConfig.ConfigurationItems.CollectionTemplate));
            container.RegisterType<ICosmosDBRepository<Category>, CosmosDBRepository<Category>>(new ContainerControlledLifetimeManager(), new InjectionConstructor(
                AppConfig.ConfigurationItems.EndPoint,
                AppConfig.ConfigurationItems.AuthKey,
                AppConfig.ConfigurationItems.Database,
                AppConfig.ConfigurationItems.CollectionTemplate));
            container.RegisterType<IUserProfileService, UserProfileService>(new PerRequestLifetimeManager(), new InjectionConstructor(
                AuthenticationConfig.ConfigurationItems.GraphAppUri,
                AuthenticationConfig.ConfigurationItems.ClientId,
                AuthenticationConfig.ConfigurationItems.AppKey,
                AuthenticationConfig.ConfigurationItems.AADInstance));

            container.RegisterType<IBlobStorageRepository, BlobStorageRepository>(new ContainerControlledLifetimeManager(), new InjectionConstructor(
                AppConfig.ConfigurationItems.BlobConnectionString));

            config.DependencyResolver = new UnityResolver(container);

            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
        }
    }
}
