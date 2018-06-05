using Android.Graphics;
using Android.Graphics.Drawables;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using QXFUtilities;
using QXFUtilities.Controls;
using QXFUtilities.Droid;

using XFColor = Xamarin.Forms.Color;


[assembly: ExportRenderer(typeof(QXFUtilities.Controls.ExtendedBoxView), typeof(QXFUtilities.Controls.Droid.ExtendedBoxViewRenderer))]
namespace QXFUtilities.Controls.Droid
{
    public class ExtendedBoxViewRenderer : BoxRenderer
    {

        public ExtendedBoxViewRenderer()
        {
            this.SetWillNotDraw(false);  // This turns back on calls to Draw (as Android interprets them) so the overridden method will be called
        }

        /// <summary>
        /// This method overrides the BoxRenderers Draw method, to draw on the canvas
        /// </summary>
        /// <param name="canvas">The passed canvas on which the control is drawn</param>
        public override void Draw(Canvas canvas)  // Canvas is the object that will be drawn on - effectively it is the Control
        {
            ExtendedBoxView ebv = (ExtendedBoxView)this.Element;

            GradientDrawable box = CreateBox(ebv.Color, ebv.CornerRadius, ebv.BorderColor, ebv.BorderThickness, ebv.ForcedBoxShape);

            // Create shadow if required
            if (ebv.HasShadow)
            {
                GradientDrawable shadow = CreateShadow(ebv.ShadowColor, ebv.CornerRadius, ebv.BorderThickness,
                                             ebv.ShadowPosition.dX, ebv.ShadowPosition.dY, ebv.Color.A, ebv.BorderColor.A, ebv.ForcedBoxShape);
                // Draw shadow on canvas
                shadow.Draw(canvas);
            }

            // Draw box on canvas
            box.Draw(canvas);  
                      
            if (ebv.Android_Elevation > 0)
            {
                this.Elevation = (float)ebv.Android_Elevation;
            }              
        }

        /// <summary>
        /// Called when a bindable property changes
        /// </summary>
        /// <param name="sender">Object: The object calling the method</param>
        /// <param name="e">PropertyChangedEventArg: Contains the name of the property changed</param>
        protected override void OnElementPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);

            if ((e.PropertyName == ExtendedBoxView.CornerRadiusProperty.PropertyName) ||
                (e.PropertyName == ExtendedBoxView.BorderColorProperty.PropertyName) ||
                (e.PropertyName == ExtendedBoxView.BorderThicknessProperty.PropertyName) ||
                (e.PropertyName == ExtendedBoxView.ColorProperty.PropertyName) ||
                (e.PropertyName == ExtendedBoxView.HasShadowProperty.PropertyName) ||
                (e.PropertyName == ExtendedBoxView.ShadowColorProperty.PropertyName) ||
                (e.PropertyName == ExtendedBoxView.ShadowPositionProperty.PropertyName) ||
                (e.PropertyName == ExtendedBoxView.ForcedBoxShapeProperty.PropertyName)
                )
            {
                this.Invalidate();          // Causes the Draw method to be called
            }
        }


        /// <summary>
        /// This methods creates a Drawable of the main box control
        /// </summary>
        /// <param name="boxColour">Xamarin.Forms.Color: Colour of the box to be drawn</param>
        /// <param name="cornerRadius">CornerRadius: Collection of the radii of the corners of the box</param>
        /// <param name="boxBorderColour">Xamarin.Forms.Color: Xamarin.Forms.Color: Colour of the box border</param>
        /// <param name="boxBorderThickness">Double: Thickness of the box border</param>
        /// <returns>GradientDrawable</returns>
        private GradientDrawable CreateBox(XFColor boxColour, CornerRadius cornerRadius, XFColor boxBorderColour, double boxBorderThickness, ForcedBoxShape forcedBoxShape)
        {
            // Create Box
            GradientDrawable box = new GradientDrawable();
            box.SetShape(ShapeType.Rectangle);
            box.SetColor(boxColour.ToAndroid());
            box.SetStroke((int)boxBorderThickness, boxBorderColour.ToAndroid());

            // Get Rectangle area to use for the main box and set the bounds of the drawable
            Rect rect = new Rect();
            GetDrawingRect(rect);

            if (forcedBoxShape != ForcedBoxShape.Default)
            {
                rect = GetCenteredSquareRect(rect);
                if (forcedBoxShape == ForcedBoxShape.Circle)
                {
                    // if its a circle, force all the corner radii to be equal to half the width
                    box.SetCornerRadius(rect.Width() / 2);
                }
                else
                {
                    box.SetCornerRadii(new float[] {(float)cornerRadius.TopLeft, (float)cornerRadius.TopLeft,
                                                    (float)cornerRadius.TopRight, (float)cornerRadius.TopRight,
                                                    (float)cornerRadius.BottomRight, (float)cornerRadius.BottomRight,
                                                    (float)cornerRadius.BottomLeft, (float)cornerRadius.BottomLeft });
                }
            }
            else
            {
                box.SetCornerRadii(new float[] {(float)cornerRadius.TopLeft, (float)cornerRadius.TopLeft,
                                                (float)cornerRadius.TopRight, (float)cornerRadius.TopRight,
                                                (float)cornerRadius.BottomRight, (float)cornerRadius.BottomRight,
                                                (float)cornerRadius.BottomLeft, (float)cornerRadius.BottomLeft });
            }

            box.Bounds = rect;
            return box;
        }

        private Rect GetCenteredSquareRect(Rect rect)
        {
            var x = rect.Left;
            var y = rect.Top;
            var w = rect.Right - x;
            var h = rect.Bottom - y;

            var rad = Math.Min(w, h);
            if (w > h)
            {
                x = x + ((w - rad) / 2);
            }
            if (h > w)
            {
                y = y + ((h - rad) / 2);
            }
            return new Rect(x, y, x + rad, y + rad);
        }


        /// <summary>
        /// This methods creates a Drawable of the main box control shadow
        /// </summary>
        /// <param name="shadowColour">Xamarin.Forms.Color: </param>
        /// <param name="cornerRadius">CornerRadius: Collection of the radii of the corners of the box</param>
        /// <param name="boxBorderThickness">Double: Thickness of the box border</param>
        /// <param name="xDeltaPosition">Double: X translation of shadow in relation to box</param>
        /// <param name="yDeltaPosition">Double: Y translation of shadow in relation to box</param>
        /// <param name="boxColourAlpha">Double: Alpha (opacity) of the box colour</param>
        /// <param name="boxBorderAlpha">Double: Alpha (opacity) of the box border colour</param>
        /// <returns>GradientDrawable</returns>
        private GradientDrawable CreateShadow(XFColor shadowColour, CornerRadius cornerRadius, double boxBorderThickness,
                                        double xDeltaPosition, double yDeltaPosition, double boxColourAlpha, double boxBorderAlpha, ForcedBoxShape forcedBoxShape)
        {
            GradientDrawable shadow = new GradientDrawable();
            shadow.SetShape(ShapeType.Rectangle);
            //Adjust the shadow color to account for any transparency in the box.
            XFColor shadowColor = new XFColor(shadowColour.R, shadowColour.G, shadowColour.B, shadowColour.A * boxColourAlpha);
            XFColor shadowBorderColor = new XFColor(shadowColour.R, shadowColour.G, shadowColour.B, shadowColour.A * boxBorderAlpha);
            shadow.SetColor(shadowColor.ToAndroid());
            shadow.SetStroke((int)boxBorderThickness, shadowBorderColor.ToAndroid());

            // Get Rectangle area to use for the shadow, offset it, and set the bounds of the drawable
            Rect rectS = new Rect();
            GetDrawingRect(rectS);

            if (forcedBoxShape != ForcedBoxShape.Default)
            {
                rectS = GetCenteredSquareRect(rectS);
                if (forcedBoxShape == ForcedBoxShape.Circle)
                {
                    // if its a circle, force all the corner radii to be equal to half the width
                    shadow.SetCornerRadius(rectS.Width() / 2);
                }
                else
                {
                    shadow.SetCornerRadii(new float[] {(float)cornerRadius.TopLeft, (float)cornerRadius.TopLeft,
                                                    (float)cornerRadius.TopRight, (float)cornerRadius.TopRight,
                                                    (float)cornerRadius.BottomRight, (float)cornerRadius.BottomRight,
                                                    (float)cornerRadius.BottomLeft, (float)cornerRadius.BottomLeft });
                }
            }
            else
            {
                shadow.SetCornerRadii(new float[] {(float)cornerRadius.TopLeft, (float)cornerRadius.TopLeft,
                                                (float)cornerRadius.TopRight, (float)cornerRadius.TopRight,
                                                (float)cornerRadius.BottomRight, (float)cornerRadius.BottomRight,
                                                (float)cornerRadius.BottomLeft, (float)cornerRadius.BottomLeft });
            }

            rectS.Offset((int)System.Math.Round(xDeltaPosition), (int)System.Math.Round(yDeltaPosition));
            shadow.Bounds = rectS;
            return shadow;
        }

    }
}