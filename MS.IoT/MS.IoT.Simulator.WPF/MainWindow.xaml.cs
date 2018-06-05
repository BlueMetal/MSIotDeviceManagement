using System.Windows;
using Microsoft.Practices.Unity;
using MS.IoT.Simulator.WPF.ViewModels;
using MS.IoT.Common;

namespace MS.IoT.Simulator.WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private MainWindowViewModel _ViewModel;

        [Dependency]
        public MainWindowViewModel ViewModel
        {
            get
            {
                return _ViewModel;
            }
            set {
                DataContext = value;
                _ViewModel = value;
            }
        }

        public MainWindow()
        {
            InitializeComponent();
            Log.InitWpf();
        }
    }
}
