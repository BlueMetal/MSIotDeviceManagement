using System;
using System.Collections.Generic;
using System.Text;

namespace QXFUtilities.DeviceInfo
{

    public enum ScreenDensityCatagory
    {
        Undefined,
        PixelsPerInch,
        PointsPerInch,
        PixelsPerPoint,
        PixeslPerViewPixel, // UWP
        Scale           // iOS scale factor - linear pixels per point
    }

    public enum DisplayCatagory
    {
        Undefined,
        Ldpi,
        Mdpi,
        Tvdpi,
        Hdpi,
        Xhdpi,
        Xxhdpi,
        Xxxhdpi,
        Xxxxhdpi,
        Retina
    }

    public enum DisplayUnit
    {
        Dip,
        Dp,
        Pt,
        Px,
        Sp,
        VPx
    }

}
