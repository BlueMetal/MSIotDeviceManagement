using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace MS.IoT.Simulator.WPF.Helpers
{
    /// <summary>
    /// NullToVisibilityConverter
    /// Converter used to convert a null value to collapsed/visible
    /// </summary>
    public sealed class NullToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value == null ? Visibility.Collapsed : Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
