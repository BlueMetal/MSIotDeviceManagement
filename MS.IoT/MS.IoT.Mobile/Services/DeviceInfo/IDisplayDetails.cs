using System;
using System.Collections.Generic;
using System.Text;

namespace QXFUtilities.DeviceInfo
{
    public interface IDisplayDetails
    {
        //Display Info
        int DisplayHeightPx { get; }
        int DisplayWidthPx { get; }

        // iOS only
        float DisplayHeightPoints { get; }
        float DisplayWidthPoints { get; }

        // Android only
        int DisplayHeightDip { get; }
        int DisplayWidthDip { get; }

        //UWP only
        int DisplayHeightViewPx { get; }
        int DisplayWidthViewPx { get; }





/*
        float DisplayHeightInches { get; }
        float DisplayWidthInches { get; }

        float DisplayHeightCm { get; }
        float DisplayWidthCm { get; }
*/

        float ScreenDensity { get; }

        ScreenDensityCatagory ScreenDensityCatagory { get; }


        int DisplaySmallestDimensionPx { get; }
        int DisplayLargestDimensionPx { get; }


        float DisplayXToYScaling { get; }


        string ToString();
    }





}
