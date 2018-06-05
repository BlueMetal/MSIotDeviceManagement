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
    public class IconNameToImageSourceConverter : IValueConverter
    {

        public const string IconsRelFilePath = "Icons/";
        public const string IconsRelResourcePath = ".Assets.Icons.";
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            string fileName = value as string;
            if (fileName == null)
                return null;

            string iconsRelPath = IconsRelFilePath;
            string imageSourceType = parameter as string;
            if (string.IsNullOrWhiteSpace(imageSourceType) || imageSourceType == "File" || imageSourceType == "file")
            {
                iconsRelPath = IconsRelFilePath;
                return GetFileImageSource(fileName, iconsRelPath);
            }
            else
            {
                if (imageSourceType == "Resource" || imageSourceType == "resource")
                    iconsRelPath = System.Reflection.Assembly.GetEntryAssembly().GetName().Name + IconsRelResourcePath;
                else
                    iconsRelPath = imageSourceType + IconsRelResourcePath;
            }
            return GetResourceImageSource(fileName, iconsRelPath);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }


        private static ImageSource GetFileImageSource(string fileName, string relPath)
        {
            string iconPath = NameToImageSourceConverter.GetImageFilePath(fileName, relPath);
            return NameToImageSourceConverter.GetImageSourceFromFile(iconPath);
        }
        private static ImageSource GetResourceImageSource(string fileName, string relPath)
        {
            string iconPath = NameToImageSourceConverter.GetImageResourcePath(fileName, relPath);
            return NameToImageSourceConverter.GetImageSourceFromResource(iconPath);
        }


    }
}
