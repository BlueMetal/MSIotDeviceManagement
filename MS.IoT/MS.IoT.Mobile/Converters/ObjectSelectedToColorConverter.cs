using System;
using Xamarin.Forms;

namespace QXFUtilities
{
    public class ObjectSelectedToColorConverter : IValueConverter //, IMarkupExtension
    {
        private static Color DefaultTrueColour { set; get; } = Color.Default;
        private static Color DefaultFalseColour { set; get; } = Color.Default;


        private Color? _trueColour = null;
        private Color? _falseColour = null;

        public Color TrueColour
        {
            get
            {
                return _trueColour == null ? DefaultTrueColour : (Color)_trueColour;        
            }
            set
            {
                _trueColour = value;
            }
        }
        public Color FalseColour
        {
            get
            {
                return _falseColour == null ? DefaultFalseColour : (Color)_falseColour;        
            }
            set
            {
                _falseColour = value;
            }
        }

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is int && parameter is string)
            {
                return ((int)value).ToString() == (string)parameter ? TrueColour : FalseColour;
            }
            if (value is string && parameter is int)
            {
                return (string)value == ((int)parameter).ToString() ? TrueColour : FalseColour;
            }
            if (value is string && parameter is string)
            { 
                return (string)value == (string)parameter ? TrueColour : FalseColour;
            }
            else if (value is int && parameter is int)
            {
                return (string)value == (string)parameter ? TrueColour : FalseColour;
            }
            else if (value is bool)
            {
                return (bool)value ? TrueColour : FalseColour;
            }
            return TrueColour;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }


/*
        public object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }
*/
    }
}
