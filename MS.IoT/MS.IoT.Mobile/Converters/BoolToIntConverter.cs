using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace QXFUtilities
{
    public class BoolToIntConverter : IValueConverter //, IMarkupExtension
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is bool)
            {
                return (bool)value ? 1 : 0;
            }
            return 1;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is int)
            {
                return (int)value > 0 ? true : false;
            }
            if (value is double)
            {
                return (double)value > 0 ? true : false;
            }
            if (value is float)
            {
                return (float)value > 0 ? true : false;
            }
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
