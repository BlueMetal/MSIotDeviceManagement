using System;
using System.Globalization;

using Xamarin.Forms;


using UWPCornerRadius = Windows.UI.Xaml.CornerRadius;
using XFCornerRadius = QXFUtilities.CornerRadius;

namespace QXFUtilities.UWP
{
    public class CornerRadiusConverter : IValueConverter
    {
        public object Convert(object xfCornerRadius, Type uwpCornerRadiusType, object parameter, CultureInfo culture)
        {
            return Convert((XFCornerRadius)xfCornerRadius);
        }

        public object ConvertBack(object uwpCornerRadius, Type xfCornerRadiusType, object parameter, CultureInfo culture)
        {
            return Convert((UWPCornerRadius)uwpCornerRadius);
        }

        public static UWPCornerRadius Convert(XFCornerRadius xfCornerRadius)
        {
            return new UWPCornerRadius(xfCornerRadius.TopLeft, xfCornerRadius.TopRight, xfCornerRadius.BottomRight, xfCornerRadius.BottomLeft);
        }
        public static XFCornerRadius Convert(UWPCornerRadius uwpCornerRadius)
        {
            return new XFCornerRadius(uwpCornerRadius.TopLeft, uwpCornerRadius.TopRight, uwpCornerRadius.BottomRight, uwpCornerRadius.BottomLeft);
        }
    }
}
