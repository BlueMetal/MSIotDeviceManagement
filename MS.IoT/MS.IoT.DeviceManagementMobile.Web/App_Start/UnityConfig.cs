using Microsoft.Practices.Unity;
using MS.IoT.DeviceManagementMobile.Web.Helpers;
using MS.IoT.Domain.Interface;
using MS.IoT.Domain.Model;
using MS.IoT.Repositories;
using System;
using System.Web.Http;
using Unity.WebApi;

namespace MS.IoT.DeviceManagementMobile.Web
{
    public class UnityConfig
    {
        #region Unity Container
        private static Lazy<IUnityContainer> container = new Lazy<IUnityContainer>(() =>
        {
            var container = new UnityContainer();
            RegisterTypes(container);
            return container;
        });

        /// <summary>
        /// Gets the configured Unity container.
        /// </summary>
        public static IUnityContainer GetConfiguredContainer()
        {
            return container.Value;
        }
        #endregion

        /// <summary>Registers the type mappings with the Unity container.</summary>
        /// <param name="container">The unity container to configure.</param>
        /// <remarks>There is no need to register concrete types such as controllers or API controllers (unless you want to 
        /// change the defaults), as Unity allows resolving a concrete type even if it was not previously registered.</remarks>
        public static void RegisterTypes(IUnityContainer container)
        {
            // NOTE: To load from web.config uncomment the line below. Make sure to add a Microsoft.Practices.Unity.Configuration to the using statements.
            // container.LoadConfiguration();

            // TODO: Register your types here
            container.RegisterType<IUserDeviceTwinRepository, UserDeviceTwinRepository>(
                new HierarchicalLifetimeManager(), new InjectionConstructor(
                AppConfig.ConfigurationItems.IoTHubConnectionString
                ));

            container.RegisterType<IDeviceTwinRepository, DeviceTwinRepository>(
               new HierarchicalLifetimeManager(), new InjectionConstructor(
               AppConfig.ConfigurationItems.IoTHubConnectionString
               ));

            container.RegisterType<INotificationHubRepository, NotificationHubRepository>(
               new HierarchicalLifetimeManager(), new InjectionConstructor(
               AppConfig.ConfigurationItems.NotificationHubConnectionString
               ));
        }
    }
}