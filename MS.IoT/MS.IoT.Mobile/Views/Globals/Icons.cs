using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using XFDevice = Xamarin.Forms.Device;



namespace MS.IoT.Mobile.GlobalResources
{
    public static class Icons
    {

        public const string IconsRelPath = "Icons/";



        public static ImageSource GetIconImageSource(string fileName)
        {
            string iconPath = GetIconPath(fileName);
            if (fileName.EndsWith(".svg"))
            {
                //  Library call to return ImageSource from SVG
                return FFImageLoading.Svg.Forms.SvgImageSource.FromFile(iconPath);
            }
            else
            {
                return ImageSource.FromFile(iconPath);
            }
        }
        public static string GetIconPath(string fileName)
        {

            switch (XFDevice.RuntimePlatform)
            {
                case XFDevice.iOS:
                    return IconsRelPath + fileName;
                case XFDevice.macOS:
                    return IconsRelPath + fileName;
                case XFDevice.Android:
                    return fileName;
                case XFDevice.UWP:
                    return IconsRelPath + fileName;
                case XFDevice.WinPhone:
                    return IconsRelPath + fileName;
                case XFDevice.WinRT:
                    return IconsRelPath + fileName;
                default:
                    return IconsRelPath + fileName;
            }
        }



    }




}
