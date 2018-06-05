using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace QXFUtilities
{
    // This supports File Resources and Embedded Resources and
    // it supports svg and native files types such as png and jpg
    public class NameToImageSourceConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is string)
            {
                return GetImageSource((string)value, parameter);
            }
            return null;
        }


        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }


        public static ImageSource GetImageSource(string pathName, object parameter = null)
        {

            if (parameter != null)
            {
                string imageSourceType = parameter as string;
                if (imageSourceType == "Resource" || imageSourceType == "resource")
                    return GetImageSourceFromResource(pathName);
            }
            return GetImageSourceFromFile(pathName);
        }


        public static ImageSource GetImageSourceFromFile(string pathName)
        {
            if (pathName.EndsWith(".svg"))
                // library to return forms image source from SVG
                return FFImageLoading.Svg.Forms.SvgImageSource.FromFile(pathName);
            else
                return ImageSource.FromFile(pathName);
        }
        public static ImageSource GetImageSourceFromResource(string pathName)
        {
            if (pathName.EndsWith(".svg"))
                // library to return forms image source from SVG
                return FFImageLoading.Svg.Forms.SvgImageSource.FromResource(pathName);
            else
                return ImageSource.FromResource(pathName);
        }



        public static string GetImagePath(string fileName, string relativeLocation, string imageSourceType = null)
        {
            if (imageSourceType == "Resource" || imageSourceType == "resource")
                return GetImageResourcePath(fileName, relativeLocation);
            else
                return GetImageFilePath(fileName, relativeLocation);
        }



        public static string GetImageFilePath(string fileName, string relativeLocation)
        {
            switch (Device.RuntimePlatform)
            {
                case Device.iOS:
                    return relativeLocation + fileName;
                case Device.macOS:
                    return relativeLocation + fileName;
                case Device.Android:
                    return fileName;
                case Device.UWP:
                    return relativeLocation + fileName;
                case Device.WinPhone:
                    return relativeLocation + fileName;
                case Device.WinRT:
                    return relativeLocation + fileName;
                default:
                    return relativeLocation + fileName;
            }
        }

        public static string GetImageResourcePath(string fileName, string relativeLocation)
        {
            return relativeLocation.Replace("/", ".") + fileName;
        }


    }
}
