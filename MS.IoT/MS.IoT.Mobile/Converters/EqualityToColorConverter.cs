using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace QXFUtilities
{
    public class EqualityToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (parameter.GetType() == typeof (Tuple<object, Color, Color>))
            {
                var tuple = (Tuple<object, Color, Color>)parameter;
                    var compareValue = tuple.Item1;
                    EqualityConverter ec = new EqualityConverter();
                    bool same = (bool)ec.Convert(value, typeof(bool), compareValue, null);
                    if (same)
                    {
                        return tuple.Item2;
                    }
                    else
                    {
                        return tuple.Item3;
                    }               
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }




    }
}
