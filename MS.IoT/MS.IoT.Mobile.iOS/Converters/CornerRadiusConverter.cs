using System;
using System.Globalization;

using Xamarin.Forms;

using XFCornerRadius = QXFUtilities.CornerRadius;

namespace QXFUtilities.iOS
{
    public class CornerRadiusConverter : IValueConverter
    {
        public object Convert(object xfCornerRadius, Type targetType, object parameter, CultureInfo culture)
        {
            if (targetType == typeof(double))
            {
                return ConvertToDouble((CornerRadius)xfCornerRadius);
            }
            if (targetType == typeof(float))
            {
                return (float)ConvertToDouble((CornerRadius)xfCornerRadius);
            }
            // any others here

            return xfCornerRadius;
        }

        public object ConvertBack(object radius, Type xfCornerRadiusType, object parameter, CultureInfo culture)
        {
            if (radius.GetType() == typeof(double) || radius.GetType() == typeof(float))
            {
                return ConvertFromDouble((double)radius);
            }
            return radius;
        }

        public static double ConvertToDouble(XFCornerRadius xfCornerRadius)
        {
            return (xfCornerRadius.TopLeft + xfCornerRadius.TopRight + xfCornerRadius.BottomRight + xfCornerRadius.BottomLeft) / 4;
         }
        public static XFCornerRadius ConvertFromDouble(double uniformRadius)
        {
            return new XFCornerRadius(uniformRadius);
        }
        public static XFCornerRadius ConvertFromDouble(double topLeft, double topRight, double bottomRight, double bottomLeft)
        {
            return new XFCornerRadius(topLeft, topRight, bottomRight, bottomLeft);
        }

    }
}
