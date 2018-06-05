using MS.IoT.Common;
using MS.IoT.Simulator.WPF.ViewModels.Interfaces;

namespace MS.IoT.Simulator.WPF.ViewModels
{
    /// <summary>
    /// Home ModelView
    /// </summary>
    public class HomeViewModel : ObservableViewModel, IHomeViewModel
    {
        /// <summary>
        /// Main Constructor
        /// </summary>
        public HomeViewModel()
        {
            Log.Debug("Home ViewModel called");
        }
    }
}
