using Microsoft.Practices.Unity;
using MS.IoT.Domain.Interface;
using MS.IoT.Domain.Model;
using MS.IoT.Repositories;
using MS.IoT.Simulator.WPF.Helpers;
using MS.IoT.Simulator.WPF.Models;
using MS.IoT.Simulator.WPF.Services;
using MS.IoT.Simulator.WPF.Services.Interfaces;
using MS.IoT.Simulator.WPF.ViewModels;
using MS.IoT.Simulator.WPF.ViewModels.Interfaces;
using System;
using System.Windows;

namespace MS.IoT.Simulator.WPF
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private IUnityContainer _Container;

        protected override void OnStartup(StartupEventArgs e)
        {
            if (string.IsNullOrEmpty(AppConfig.CosmosDB.EndPoint) || string.IsNullOrEmpty(AppConfig.CosmosDB.AuthKey))
            {
                MessageBox.Show("The properties EndPoint and/or Authorization key of Cosmos DB are not set up. Please make sure to indicate these values in the App.config of the Simulator.");
                Environment.Exit(0);
                return;
            }

            _Container = new UnityContainer();
            _Container.RegisterType<ICosmosDBRepository<Template>, CosmosDBRepository<Template>>(new ContainerControlledLifetimeManager(), new InjectionConstructor(
               AppConfig.CosmosDB.EndPoint,
               AppConfig.CosmosDB.AuthKey,
               AppConfig.CosmosDB.Database,
               AppConfig.CosmosDB.CollectionTemplate));
            _Container.RegisterType<ICosmosDBRepository<Category>, CosmosDBRepository<Category>>(new ContainerControlledLifetimeManager(), new InjectionConstructor(
                AppConfig.CosmosDB.EndPoint,
                AppConfig.CosmosDB.AuthKey,
                AppConfig.CosmosDB.Database,
                AppConfig.CosmosDB.CollectionTemplate));
            _Container.RegisterType<ICosmosDBRepository<CosmosDBMessage>, CosmosDBRepository<CosmosDBMessage>>(new ContainerControlledLifetimeManager(), new InjectionConstructor(
                AppConfig.CosmosDB.EndPoint,
                AppConfig.CosmosDB.AuthKey,
                AppConfig.CosmosDB.Database,
                AppConfig.CosmosDB.CollectionMessage));

            _Container.RegisterType<IUserService, ActiveDirectoryUserService>(new ContainerControlledLifetimeManager());
            _Container.RegisterType<IFourByFourService, FourByFourService>(new ContainerControlledLifetimeManager());
            _Container.RegisterType<IIoTHubService, IoTHubService>(new ContainerControlledLifetimeManager());

            _Container.RegisterType<IHomeViewModel, HomeViewModel>(new ContainerControlledLifetimeManager());
            _Container.RegisterType<ISelectTemplateViewModel, SelectTemplateViewModel>(new ContainerControlledLifetimeManager());
            _Container.RegisterType<IPreviewTemplateViewModel, PreviewTemplateViewModel>(new ContainerControlledLifetimeManager());
            _Container.RegisterType<ISimulateTemplateViewModel, SimulateTemplateViewModel>(new ContainerControlledLifetimeManager());

            _Container.Resolve<MainWindow>().Show();
        }

        protected void App_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            MessageBox.Show("An unhandled exception just occurred: " + e.Exception.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            e.Handled = true;
        }

        protected override void OnExit(ExitEventArgs e)
        {
            _Container.Dispose();
            base.OnExit(e);
        }
    }
}
