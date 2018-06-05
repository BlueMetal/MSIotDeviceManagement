using System;
using System.ComponentModel;
using System.Windows.Input;
using System.Diagnostics;

using Xamarin.Forms;
using System.Runtime.CompilerServices;

namespace QXFUtilities
{
    public class BaseViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public BaseViewModel ParentViewModel { get; set; } = null;



        public BaseViewModel()
        {

        }
        public BaseViewModel(BaseViewModel parentViewModel)
        {
            ParentViewModel = parentViewModel;
        }


        protected void OnPropertyChanged<T>([CallerMemberName]string propertyName = null)
        {
            var handler = PropertyChanged;
            if ((handler != null) && (propertyName != null))
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        /*
        *** Usage ***
        private SomeClass _someProperty = null (or whatever default value)
        public SomeClass SomeProperty
        {
            get
            {
                return _someProperty;
            }
            set
            {
                if (value != _someProperty)
                {
                    _someProperty = value;
                    OnPropertyChanged<SomeClass>();
                }
            }
        }
        */

        protected void SetProperty<T>(T value, ref T backingField)
        {
            backingField = value;
            OnPropertyChanged<T>();
        }
        //*** Usage ***
        // private SomeClass _someProperty = null (or whatever default value);
        //.......
        // SetPropert<SomeClass>(value, ref _someProperty)


        // Override this to handle row selections
        public virtual void OnRowSelected(object sender, object item)
        {


        }

    }
}
