using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace QXFUtilities
{
    public class NonEqualityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            Type vType = value.GetType();
            Type pType = parameter.GetType();

            if (vType == pType)
            {
                return !value.Equals(parameter);
            }
            else
            {
                if (pType == typeof(string))
                {
                    // if the parameter type is a string, it might be a XAML casting issue having changed type so lets try to cast back
                    if (vType == typeof(int))
                    {
                        try
                        {
                            return !value.Equals((int)parameter);
                        }
                        catch { return true; }
                    }
                    else if (vType == typeof(long))
                    {
                        try
                        {
                            return !value.Equals((long)parameter);
                        }
                        catch { return true; }
                    }
                    else if (vType == typeof(short))
                    {
                        try
                        {
                            return !value.Equals((short)parameter);
                        }
                        catch { return true; }
                    }
                    else if (vType == typeof(double))
                    {
                        try
                        {
                            return !value.Equals((double)parameter);
                        }
                        catch { return true; }
                    }
                    else if (vType == typeof(float))
                    {
                        try
                        {
                            return !value.Equals((float)parameter);
                        }
                        catch { return true; }
                    }
                    else if (vType == typeof(byte))
                    {
                        try
                        {
                            return !value.Equals((byte)parameter);
                        }
                        catch { return true; }
                    }
                    else if (vType == typeof(decimal))
                    {
                        try
                        {
                            return !value.Equals((decimal)parameter);
                        }
                        catch { return true; }
                    }
                    else if (vType == typeof(sbyte))
                    {
                        try
                        {
                            return !value.Equals((sbyte)parameter);
                        }
                        catch { return true; }
                    }
                    else if (vType == typeof(uint))
                    {
                        try
                        {
                            return !value.Equals((uint)parameter);
                        }
                        catch { return true; }
                    }
                    else if (vType == typeof(ulong))
                    {
                        try
                        {
                            return !value.Equals((ulong)parameter);
                        }
                        catch { return true; }
                    }
                    else if (vType == typeof(ushort))
                    {
                        try
                        {
                            return !value.Equals((ushort)parameter);
                        }
                        catch { return true; }
                    }
                }
            }
            return true;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}