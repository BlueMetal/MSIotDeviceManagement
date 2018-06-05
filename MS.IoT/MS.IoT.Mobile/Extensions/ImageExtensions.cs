using System;
using System.Collections.Generic;
using System.Text;

using Xamarin.Forms;


namespace Xamarin.Forms
{
    public static class ImageExtensions
    {

        public static ImageSource FromSVGFile(string svgFilePath)
        {
            return FFImageLoading.Svg.Forms.SvgImageSource.FromFile(svgFilePath);
        }



    }
}
