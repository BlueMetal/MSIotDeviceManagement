using MS.IoT.UWP.CoffeeMaker.Helpers;
using MS.IoT.UWP.CoffeeMaker.Models;
using MS.IoT.UWP.CoffeeMaker.Models.Packets;
using MS.IoT.UWP.CoffeeMaker.Services;
using MS.IoT.UWP.CoffeeMaker.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace MS.IoT.UWP.CoffeeMaker
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainWindowViewModel ViewModel { get; set; }
        

        /// <summary>
        /// Main Constructor
        /// </summary>
        public MainPage()
        {
            this.Unloaded += MainPage_Unloaded;

            //Set up log
            LogHelper.LogEntry += LogHelper_LogEntry;

            //Set up model
            ViewModel = new MainWindowViewModel();
            ViewModel.IsPanelLoadingVisible = true;
            this.DataContext = ViewModel;

            this.InitializeComponent();
        }

        private void LogHelper_LogEntry(object sender, string e)
        {
            if (ViewModel == null || !ViewModel.IsFeatureDebugActivated)
                return;
            ViewModel.DebugUIConsole += e;
        }

        private void MainPage_Unloaded(object sender, RoutedEventArgs e)
        {
            if (ViewModel != null)
                ViewModel.Dispose();
        }

        private void ActionBrewStrength_Tapped(object sender, TappedRoutedEventArgs e)
        {
            if(!ViewModel.IsActionBrewLaunched && !ViewModel.IsActionGrindAndBrewLaunched)
                ViewModel.LaunchActionBrewStrength();
        }

        private void ActionBrew_Tapped(object sender, TappedRoutedEventArgs e)
        {
            ViewModel.LaunchActionBrew();
        }

        private void ActionGrindBrew_Tapped(object sender, TappedRoutedEventArgs e)
        {
            ViewModel.LaunchActionGrindBrew();
        }

        private void WifiIcon_Tapped(object sender, TappedRoutedEventArgs e)
        {
            ViewModel.OpenWifiPanel();
        }

        private void BtnWifiCancel_Tapped(object sender, TappedRoutedEventArgs e)
        {
            ViewModel.CloseWifiPanel();
        }

        private void BtnWifiConnect_Tapped(object sender, TappedRoutedEventArgs e)
        {
            ViewModel.LaunchActionSetWifi();
        }

        private void BtnDebug_Tapped(object sender, TappedRoutedEventArgs e)
        {
            ViewModel.OpenDebugPanel();
        }

        private void BtnDebugClose_Tapped(object sender, TappedRoutedEventArgs e)
        {
            ViewModel.CloseDebugPanel();
        }
    }
}
