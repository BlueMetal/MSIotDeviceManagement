using System.ComponentModel;
using System.Runtime.CompilerServices;


namespace QXFUtilities
{
    public class BaseBindableProperty : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

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


    }
}
