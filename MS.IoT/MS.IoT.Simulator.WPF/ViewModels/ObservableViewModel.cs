using System;
using System.ComponentModel;

namespace MS.IoT.Simulator.WPF.ViewModels
{
    /// <summary>
    /// Base class for ModelViews, implements INotifyPropertyChanged and provides OnUnload/OnLoad methods that are fired before/after navigating from one view to another.
    /// </summary>
    public abstract class ObservableViewModel : INotifyPropertyChanged, IDisposable
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


        /// <summary>
        /// Property set up while changing a view
        /// </summary>
        public object Parameter { get; set; }


        /// <summary>
        /// Method OnLoad used after swapping to a new ViewModel. This method has access to Parameter
        /// </summary>
        public virtual void OnLoad()
        {
        }

        /// <summary>
        /// Method OnUnload used just before swapping to a new ViewModel.
        /// </summary>
        public virtual void OnUnload()
        {

        }

        /// <summary>
        /// Method to be used when the application exist to free resources
        /// </summary>
        public virtual void Dispose()
        {
            
        }
    }
}
