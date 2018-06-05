using System;
using Xamarin.Forms;

namespace QXFUtilities
{
    public class NotConverter : IValueConverter //, IMarkupExtension
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is bool)
            {
                return !(bool)value;
            }
            return false;
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
