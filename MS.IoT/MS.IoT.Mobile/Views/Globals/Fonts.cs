using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

using XFDevice = Xamarin.Forms.Device;

namespace MS.IoT.Mobile.GlobalResources
{
    public static class Fonts
    {
        // Fonts

        // Platform Specific Definitions
        // iOS - Use the Postscript Name if different from the Font Name
        // UWP use the Preferred Font Family after the filename

        private const string IosSegeoUI = "SegoeUI-Regular";
        private const string AndroidSegeoUI = "Segoe_UI.ttf#SegoeUI-Regular";
        private const string UWPSegeoUI = "/Assets/Fonts/Segoe_UI.ttf#Segoe UI";

        private const string IosSegeoWP = "SegoeWP";
        private const string AndroidSegeoWP = "segeo-wp.ttf#SegoeWP";
        private const string UWPSegeoWP = "/Assets/Fonts/segeo-wp.ttf#Segoe WP";

        private const string IosSegeoWPBlack = "SegoeWP-Black";
        private const string AndroidSegeoWPBlack = "segeo-wp-black.ttf#SegoeWP-Black";
        private const string UWPSegeoWPBlack = "/Assets/Fonts/segeo-wp-black.ttf#Segoe WP Black";

        private const string IosSegeoWPBold = "SegoeWP-Bold";
        private const string AndroidSegeoWPBold = "segeo-wp-bold.ttf#SegoeWP-Bold";
        private const string UWPSegeoWPBold = "/Assets/Fonts/segeo-wp-bold.ttf#Segoe WP";

        private const string IosSegeoWPLight= "SegoeWP-Light";
        private const string AndroidSegeoWPLight = "segeo-wp-light.ttf#SegoeWP-Light";
        private const string UWPSegeoWPLight = "/Assets/Fonts/segeo-wp-light.ttf#Segoe WP Light";

        private const string IosSegeoWPN = "SegoeWPN";
        private const string AndroidSegeoWPN = "SegoeWPN.ttf#SegoeWPN";
        private const string UWPSegeoWPN = "/Assets/Fonts/SegoeWPN.ttf#Segoe WP N";

        private const string IosSegeoWPRegular = "SegoeWP";
        private const string AndroidSegeoWPRegular = "segeo-wp-webfont.ttf#SegoeWP";
        private const string UWPSegeoWPRegular = "/Assets/Fonts/segeo-wp-webfont.ttf#Segoe WP";

        private const string IosSegeoWPSemiBold = "SegoeWP-Semibold";
        private const string AndroidSegeoWPSemiBold = "segeo-wp-semibold.ttf#SegoeWP-Semibold";
        private const string UWPSegeoWPSemiBold = "/Assets/Fonts/segeo-wp-semibold.ttf#Segoe WP Semibold";


        // Application Font name Resolution
        // These use Device.RuntimePlatform to resolve the platform
        // They are access in Xaml by: xmlns:local="clr-namespace:<ProjectNamespace>;assembly=<ProjectNamespace>"
        //                             = "{x:Static local:GlobalResources.Fonts.<ResourceName>}
        public static readonly string SegeoUI = ResolveFontByPlatform(IosSegeoUI, AndroidSegeoUI, UWPSegeoUI);
        public static readonly string SegeoWP = ResolveFontByPlatform(IosSegeoWP, AndroidSegeoWP, UWPSegeoWP);
        public static readonly string SegeoWPBlack = ResolveFontByPlatform(IosSegeoWPBlack, AndroidSegeoWPBlack, UWPSegeoWPBlack);            
        public static readonly string SegeoWPBold = ResolveFontByPlatform(IosSegeoWPBold, AndroidSegeoWPBold, UWPSegeoWPBold);              
        public static readonly string SegeoWPLight = ResolveFontByPlatform(IosSegeoWPLight, AndroidSegeoWPLight, UWPSegeoWPLight);
        public static readonly string SegeoWPN = ResolveFontByPlatform(IosSegeoWPN, AndroidSegeoWPN, UWPSegeoWPN);
        public static readonly string SegeoWPRegular = ResolveFontByPlatform(IosSegeoWPRegular, AndroidSegeoWPRegular, UWPSegeoWPRegular);
        public static readonly string SegeoWPSemiBold = ResolveFontByPlatform(IosSegeoWPSemiBold, AndroidSegeoWPSemiBold, UWPSegeoWPSemiBold);


        private static string ResolveFontByPlatform(string iosName, string androidName, string uwpName = null)
        {
            switch (XFDevice.RuntimePlatform)
            {
                case XFDevice.iOS:
                    return iosName;
                case XFDevice.Android :
                    return androidName;
                case XFDevice.UWP :
                    return uwpName;
                case XFDevice.WinRT :
                    return uwpName;
                case XFDevice.WinPhone :
                    return uwpName;
                default:
                    return null;
            }
        }





    }
}
