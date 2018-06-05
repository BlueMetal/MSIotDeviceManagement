using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Foundation;
using UIKit;

using Xamarin.Forms;
using QXFUtilities.DeviceInfo;
using System.Drawing;

[assembly: Dependency(typeof(QXFUtilities.iOS.DeviceInfo.DisplayDetailsService))]
namespace QXFUtilities.iOS.DeviceInfo
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

        private float _displayHeightPoints = -1f;
        public float DisplayHeightPoints
        {
            get
            {
                if (_displayHeightPoints == -1f)
                {
                    _displayHeightPoints = GetDisplayHeightPoints();
                }
                return _displayHeightPoints;
            }
        }

        private float _displayWidthPoints = -1f;
        public float DisplayWidthPoints
        {
            get
            {
                if (_displayWidthPoints == -1f)
                {
                    _displayWidthPoints = GetDisplayWidthPoints();
                }
                return _displayWidthPoints;
            }
        }


        // Note Dip NOT used in iOS. Use Points
        public int DisplayHeightDip { get { return -1; } }
        public int DisplayWidthDip { get { return -1; } }

        // Note View Pixels NOT used in iOS. Use Points
        public int DisplayHeightViewPx { get { return -1; } }
        public int DisplayWidthViewPx { get { return -1; } }




        /*
                private float _displayHeightInches = -1f;
                public float DisplayHeightInches
                {
                    get
                    {
                        if (_displayHeightInches == -1f)
                        {
                            _displayHeightInches = GetScreenSizeInches().Height;
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
                            _displayWidthInches = GetScreenSizeInches().Width;
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
                            _displayHeightCm = GetScreenSizeCm().Height;
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
                            _displayWidthCm = GetScreenSizeCm().Width;
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



        private SizeF GetDisplaySizePoints()
        {
            return new SizeF((float)UIScreen.MainScreen.Bounds.Size.Width, (float)UIScreen.MainScreen.Bounds.Size.Height);
        }

        private float GetDisplayHeightPoints()
        {
            return (float)UIScreen.MainScreen.Bounds.Size.Height;
        }

        private float GetDisplayWidthPoints()
        {
            return (float)UIScreen.MainScreen.Bounds.Size.Width;
        }


        private SizeF GetDisplaySizePx()
        {
            return new SizeF((float)(UIScreen.MainScreen.Scale * UIScreen.MainScreen.Bounds.Size.Width), (float)(UIScreen.MainScreen.Scale * UIScreen.MainScreen.Bounds.Size.Height));
        }

        private int GetDisplayHeightPx()
        {
            return (int)(UIScreen.MainScreen.Scale * UIScreen.MainScreen.Bounds.Size.Height);  // logical pixels
        }

        private int GetDisplayWidthPx()
        {
            return (int)(UIScreen.MainScreen.Scale * UIScreen.MainScreen.Bounds.Size.Width);  // logical pixels
        }


        private float GetScreenDensity()
        {
            return (float)UIScreen.MainScreen.Scale;
        }

        private ScreenDensityCatagory GetScreenDensityCatagory()
        {
            return ScreenDensityCatagory.Scale;
        }

/*
        private SizeF GetScreenSizeInches()
        {
            float ppi = this._screenDensity * ((this._deviceCatagory == DeviceCatagory.Tablet) ? 132f : 163f);
            SizeF size = GetDisplaySizePx();
            return new SizeF(size.Width / ppi, size.Height / ppi);
        }

        private SizeF GetScreenSizeCm()
        {
            float ppi = this._screenDensity * ((this._deviceCatagory == DeviceCatagory.Tablet) ? 132f : 163f);
            SizeF size = GetDisplaySizePx();
            return new SizeF(2.54f * size.Width / ppi, 2.54f * size.Height / ppi);
        }
*/

        // Gets the devices smallest screen dimensions in pixels
        private int GetDisplaySmallestDimensionPixels()
        {
            SizeF size = GetDisplaySizePx();
            return (int)Math.Min(size.Width, size.Height);
        }

        // Gets the devices largest screen dimensions in pixels
        private int GetDisplayLargestDimensionPixels()
        {
            SizeF size = GetDisplaySizePx();
            return (int)Math.Max(size.Width, size.Height);
        }


        private float GetDisplayXYScale()
        {
            return (float)(UIScreen.MainScreen.Bounds.Size.Width / UIScreen.MainScreen.Bounds.Size.Height);
        }



        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("; DisplayWidthPx= ");
            sb.Append(this.DisplayWidthPx);
            sb.Append("; DisplayHeightPx= ");
            sb.Append(this.DisplayHeightPx);
            sb.Append("; DisplayWidthPoints= ");
            sb.Append(this.DisplayWidthPoints);
            sb.Append("; DisplayHeightPoints= ");
            sb.Append(this.DisplayHeightPoints);
            sb.Append("; ScreenDenistyCat=");
            sb.Append(this.ScreenDensityCatagory);
            sb.Append("; ScreenDensity=");
            sb.Append(this.ScreenDensity);
            return sb.ToString();
        }



    }
}