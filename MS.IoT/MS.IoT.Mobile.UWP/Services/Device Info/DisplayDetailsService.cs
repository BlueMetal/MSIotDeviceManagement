using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

using QXFUtilities.DeviceInfo;
using System.Drawing;

[assembly: Dependency(typeof(QXFUtilities.UWP.DeviceInfo.DisplayDetailsService))]
namespace QXFUtilities.UWP.DeviceInfo
{
    public class DisplayDetailsService : IDisplayDetails
    {
        //Display Info
        private int _displayHeightPx = -1;
        public int DisplayHeightPx
        {
            get
            {
                if (_displayHeightPx == -1)
                {
                    _displayHeightPx = GetDisplayHeightPx();
                }
                return _displayHeightPx;
            }
        }

        private int _displayWidthPx = -1;
        public int DisplayWidthPx
        {
            get
            {
                if (_displayWidthPx == -1)
                {
                    _displayWidthPx = GetDisplayWidthPx();
                }
                return _displayWidthPx;
            }
        }

        // point not used in UWP
        public float DisplayHeightPoints { get { return -1f; } }

        public float DisplayWidthPoints { get { return -1f; } }


        private int _displayHeightViewPx = -1;
        public int DisplayHeightViewPx
        {
            get
            {
                if (_displayHeightViewPx == -1)
                {
                    _displayHeightViewPx = GetDisplayHeightViewPx();
                }
                return _displayHeightViewPx;
            }
        }
        private int _displayWidthViewPx = -1;
        public int DisplayWidthViewPx
        {
            get
            {
                if (_displayWidthViewPx == -1)
                {
                    _displayWidthViewPx = GetDisplayWidthViewPx();
                }
                return _displayWidthViewPx;
            }
        }


       // Note DIP NOT used in UWP
        public int DisplayHeightDip { get { return -1; } }

        public int DisplayWidthDip { get { return -1; } }


/*
        private float _displayHeightInches = -1f;
        public float DisplayHeightInches
        {
            get
            {
                if (_displayHeightInches == -1f)
                {
                    _displayHeightInches = GetScreenHeightInches();
                }
                return _displayHeightInches;
            }
        }

        private float _displayWidthInches = -1f;
        public float DisplayWidthInches
        {
            get
            {
                if (_displayWidthInches == -1f)
                {
                    _displayWidthInches = GetScreenWidthInches();
                }
                return _displayWidthInches;
            }
        }


        private float _displayHeightCm = -1f;
        public float DisplayHeightCm
        {
            get
            {
                if (_displayHeightCm == -1f)
                {
                    _displayHeightCm = GetScreenHeightCm();
                }
                return _displayHeightCm;
            }
        }

        private float _displayWidthCm = -1f;
        public float DisplayWidthCm
        {
            get
            {
                if (_displayWidthCm == -1f)
                {
                    _displayWidthCm = GetScreenWidthCm();
                }
                return _displayWidthCm;
            }
        }
*/

        private float _screenDensity = -1f;
        public float ScreenDensity
        {
            get
            {
                if (_screenDensity == -1f)
                {
                    _screenDensity = GetScreenDensity();
                }
                return _screenDensity;
            }
        }


        private ScreenDensityCatagory _screenDensityCatagory = ScreenDensityCatagory.Undefined;
        public ScreenDensityCatagory ScreenDensityCatagory
        {
            get
            {
                if (_screenDensityCatagory == ScreenDensityCatagory.Undefined)
                {
                    _screenDensityCatagory = GetScreenDensityCatagory();
                }
                return _screenDensityCatagory;
            }
        }


        private int _displaySmallestDimensionPx = -1;
        public int DisplaySmallestDimensionPx
        {
            get
            {
                if (_displaySmallestDimensionPx == -1)
                {
                    _displaySmallestDimensionPx = GetDisplaySmallestDimensionPixels();
                }
                return _displaySmallestDimensionPx;
            }
        }

        private int _displayLargestDimensionPx = -1;
        public int DisplayLargestDimensionPx
        {
            get
            {
                if (_displayLargestDimensionPx == -1)
                {
                    _displayLargestDimensionPx = GetDisplayLargestDimensionPixels();
                }
                return _displayLargestDimensionPx;
            }
        }

        public float DisplayXToYScaling
        {
            get { return GetDisplayXYScale(); }

        }


        //Device Info

        public static SizeF GetDisplaySizePx()
        {
            return new SizeF(GetDisplayWidthPx(), GetDisplayHeightPx());
        }

        public static int GetDisplayHeightPx()
        {
            return (int)Windows.Graphics.Display.DisplayInformation.GetForCurrentView().ScreenWidthInRawPixels;
        }

        public static int GetDisplayWidthPx()
        {
            return (int)Windows.Graphics.Display.DisplayInformation.GetForCurrentView().ScreenHeightInRawPixels;
        }

        public static float GetScreenDensity()
        {
            return (float)Windows.Graphics.Display.DisplayInformation.GetForCurrentView().RawPixelsPerViewPixel;

        }

        public static ScreenDensityCatagory GetScreenDensityCatagory()
        {
            return ScreenDensityCatagory.PixeslPerViewPixel;
        }

        private SizeF GetDisplaySizeDip()
        {
            float sd = GetScreenDensity();
            return new SizeF(GetDisplayWidthPx() / sd, GetDisplayHeightPx() / sd);
        }

        public static int GetDisplayHeightViewPx()
        {
            return (int)Windows.UI.ViewManagement.ApplicationView.GetForCurrentView().VisibleBounds.Width;
        }

        public static int GetDisplayWidthViewPx()
        {
            return (int)Windows.UI.ViewManagement.ApplicationView.GetForCurrentView().VisibleBounds.Height;
        }

/*
        public static float GetScreenHeightInches()
        {
            return (float)(GetDisplayHeightPx() / Resources.System.DisplayMetrics.Ydpi);
        }

        public static float GetScreenWidthInches()
        {
            return (float)(GetDisplayWidthPx() / Resources.System.DisplayMetrics.Xdpi);
        }

        public static float GetScreenHeightCm()
        {
            return (float)(2.54f * GetDisplayHeightPx() / Resources.System.DisplayMetrics.Ydpi);
        }

        public static float GetScreenWidthCm()
        {
            return (float)(2.54f * GetDisplayWidthPx() / Resources.System.DisplayMetrics.Xdpi);
        }
*/

        // Gets the devices smallest screen dimensions in pixels
        public static int GetDisplaySmallestDimensionPixels()
        {
            return (int)Math.Min(GetDisplayWidthPx(), GetDisplayHeightPx());
        }

        // Gets the devices largest screen dimensions in pixels
        public static int GetDisplayLargestDimensionPixels()
        {
            return (int)Math.Max(GetDisplayWidthPx(), GetDisplayHeightPx());
        }


        private float GetDisplayXYScale()
        {
            return (float)(GetDisplayWidthPx() / GetDisplayHeightPx());
        }


        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("; DisplayWidthPx= ");
            sb.Append(this.DisplayWidthPx);
            sb.Append("; DisplayHeightPx= ");
            sb.Append(this.DisplayHeightPx);
            sb.Append("; DisplayWidthViewPx= ");
            sb.Append(this.DisplayWidthViewPx);
            sb.Append("; DisplayHeightViewPx= ");
            sb.Append(this.DisplayHeightViewPx);
            sb.Append("; ScreenDenistyCat=");
            sb.Append(this.ScreenDensityCatagory);
            sb.Append("; ScreenDensity=");
            sb.Append(this.ScreenDensity);
            return sb.ToString();
        }

    }
}
