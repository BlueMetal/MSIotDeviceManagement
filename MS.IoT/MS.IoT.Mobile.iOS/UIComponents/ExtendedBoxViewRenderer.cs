using System;
using System.Drawing;

using UIKit;
using CoreGraphics;
using CoreAnimation;

using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

using QXFUtilities;
using QXFUtilities.Controls;
using QXFUtilities.iOS;


[assembly: ExportRenderer(typeof(QXFUtilities.Controls.ExtendedBoxView), typeof(QXFUtilities.Controls.iOS.ExtendedBoxViewRenderer))]
namespace QXFUtilities.Controls.iOS
{

    public class ExtendedBoxViewRenderer : ViewRenderer<ExtendedBoxView, UIView>
    {
        protected override void OnElementChanged(ElementChangedEventArgs<ExtendedBoxView> e)
        {
            base.OnElementChanged(e);

            ExtendedBoxView ebv = e.NewElement;

            if ((ebv != null) && ebv.IsVisible) {
                // Declare the overall View, This will hold any calculated views
                UIView ebvView = CreateBoxView(ebv);

                // Add a shadow to the overall view if required
                if (ebv.HasShadow)
                {
                    // Adjust shadow colour for any transparency in the box colour
                    Color shadowColour = new Color(ebv.ShadowColor.R, ebv.ShadowColor.G, ebv.ShadowColor.B, ebv.ShadowColor.A * ebv.Color.A);
                    AddShadow(ebvView, shadowColour.ToCGColor(), (float)ebv.ShadowPosition.dX, (float)ebv.ShadowPosition.dY);
                }

                // Set to the be the Native Control
                SetNativeControl(ebvView);
            }
        }

        protected override void OnElementPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if ((e.PropertyName == ExtendedBoxView.CornerRadiusProperty.PropertyName) ||
                (e.PropertyName == ExtendedBoxView.BorderColorProperty.PropertyName) ||
                (e.PropertyName == ExtendedBoxView.BorderThicknessProperty.PropertyName) ||
                (e.PropertyName == BoxView.ColorProperty.PropertyName) ||
                (e.PropertyName == BoxView.IsVisibleProperty.PropertyName) ||
                (e.PropertyName == ExtendedBoxView.ForcedBoxShapeProperty.PropertyName))
            {
                // SetNatuve Control is additive, so remove the current view from the Native Control
                // Cycle through any sub-views until get to the correct view identified by the Tag
                // property. This is done to be defensive, rather than assume there is only one sub view
                // or if there are any sub views
                foreach (UIView subv1 in NativeView.Subviews)
                {
                    if (subv1.Tag == 1)
                    {
                        subv1.RemoveFromSuperview();
                        break;
                    }
                }

                if (this.Element.IsVisible)
                {
                    UIView ebvView = CreateBoxView(this.Element);

                    SetNativeControl(ebvView);
                }
            }
        

            // Process shadow property changes - this can be done directly on the Native View without re-assigning it
            if (e.PropertyName == ExtendedBoxView.HasShadowProperty.PropertyName)
            {
                ExtendedBoxView ebv = this.Element;
                if (ebv.HasShadow)
                {
                    // Adjust shadow colour for any transparency in the box colour
                    Color shadowColour = new Color(ebv.ShadowColor.R, ebv.ShadowColor.G, ebv.ShadowColor.B, ebv.ShadowColor.A * ebv.Color.A);
                    AddShadow(NativeView, shadowColour.ToCGColor(), (float)ebv.ShadowPosition.dX, (float)ebv.ShadowPosition.dY);
                }
                else
                {

                    RemoveShadow(NativeView);
                }
            }
            else if (e.PropertyName == ExtendedBoxView.ShadowPositionProperty.PropertyName)
            {
                ExtendedBoxView ebv = this.Element;
                if (ebv.HasShadow)
                {
                    NativeView.Layer.ShadowOffset = new CGSize((float)ebv.ShadowPosition.dX, (float)ebv.ShadowPosition.dY);
                }
            }
            else if (e.PropertyName == ExtendedBoxView.ShadowColorProperty.PropertyName)
            {
                ExtendedBoxView ebv = this.Element;
                if (ebv.HasShadow)
                {
                    // Adjust shadow colour for any transparency in the box colour
                    Color shadowColour = new Color(ebv.ShadowColor.R, ebv.ShadowColor.G, ebv.ShadowColor.B, ebv.ShadowColor.A * ebv.Color.A);
                    NativeView.Layer.ShadowColor = shadowColour.ToCGColor();
                }
            }
		}

        private UIView CreateBoxView(ExtendedBoxView ebv)
        {
            // Declare the overall View, This will hold any calculated views
            UIView ebvView = new UIView();

            if (ebv != null)
            {
                // allocate a tag value to help remove the view on updates
                ebvView.Tag = 1;

                // Create the RoundedRectangle and assign the to childView
                UIView childView = new RoundedRectView()
                {
                    BorderThickness = (float)ebv.BorderThickness,
                    BackgroundColor = ebv.Color.ToUIColor(),
                    BorderColor = ebv.BorderColor.ToUIColor(),
                    CornerRadius = ebv.CornerRadius,
                    ClipsToBounds = true,
                    AutoresizingMask = UIViewAutoresizing.FlexibleWidth | UIViewAutoresizing.FlexibleHeight,
                    ForcedBoxShape = ebv.ForcedBoxShape
                };

                // Add the view to the overall view
                ebvView.Add(childView);


            }
            return ebvView;
        }

        /// <summary>
        /// This method adds a shadow layer to the passed view
        /// </summary> 
        /// <param name='view'>UIVuiew - the view have shadow added to it</param> 
        private void AddShadow(UIView view, CGColor shadowColor, float xDeltaPosition, float yDeltaPosition)
        {
            view.Layer.ShadowColor = shadowColor; ;
            view.Layer.ShadowOffset = new CGSize(xDeltaPosition, yDeltaPosition);
            view.Layer.ShadowOpacity = 1.0f;
            // ebvView.Layer.ShadowRadius // Note: This property creates a blurred effect on the shadow - currently not supported
        }
        /// <summary>
        /// This method removes a shadow layer to the passed view
        /// </summary> 
        /// <param name='view'>UIVuiew - the view have shadow removed from it</param> 
        private void RemoveShadow(UIView view)
        {
            view.Layer.ShadowColor = UIColor.Clear.CGColor;
            view.Layer.ShadowOffset = new CGSize();
            view.Layer.ShadowOpacity = 0;
        }

	}



    /// <summary>
    /// This class creates a path and draws it on the screen via a UIContext.
    /// The path is created manually as a UIBezierPath.
    /// The size and position of the is set as a the bounds of a rectangle based upon the 
    /// bounds of the current view. As this changes during the layout passes, it must be
    /// recalculated each time Bounds, Frame or related properties are changed. It is only
    /// calculated if the layout pass is one where the bound shave been calculated
    /// </summary>
    public class RoundedRectView : UIView
    {
 
        private CGPath shapePath = null;

        public float BorderThickness { get; set; } = 0f;

        public UIColor BorderColor { get; set; } = UIColor.Clear;

        public CornerRadius CornerRadius { get; set; } = new CornerRadius(0);

        public ForcedBoxShape ForcedBoxShape { get; set; } = ForcedBoxShape.Default;

        public override UIViewAutoresizing AutoresizingMask
        {
            get
            {
                return base.AutoresizingMask;
            }
            set
            {
                base.AutoresizingMask = value;
                this.Update();
            }
        }

        public override CGRect Bounds
        {
            get
            {
                return base.Bounds;
            }
            set
            {
                base.Bounds = value;
                this.Update();
            }
        }

        public override CGRect Frame
        {
            get
            {
                return base.Frame;
            }
            set
            {
                base.Frame = value;
                this.Update();
            }
        }



        /// <summary> 
        /// Initializes a new instance of the class. 
        /// </summary> 
        public RoundedRectView() : base()
        {
            this.Update();
        }

        /// <summary> 
        /// Initializes a new instance of the class. 
        /// </summary> 
        /// <param name='rect'>rectangle of the view</param> 
        public RoundedRectView(RectangleF rect) : base(rect)
        {
            this.Update();
        }



        /// <summary> 
        /// Updates the shape path and layer's mask if required. With iOS there are no 
        /// springs/struts (no auto resizing) on CALayers, therefore the shape path and mask 
        /// have to be adjusted via a call to this method, whenever the UIView's properties 
        /// are altered, which happens several times during the layout process. The updates
        /// only occur if the views bounds have been calculated by the layout pass
        /// </summary>
        private void Update()
        {
            if (this.Bounds.Size != CGSize.Empty)
            {
                CalculatePath();
                UpdateMask();
            }
        }


        /// <summary> 
        /// Updates path shape. This is calculaTed manually
        /// Each corner will be circular. Ellipsoid corners are not currently supported.
        /// If two corners would overlap, their radii are adjusted to prevent this.
        /// Each corner is guaranteed a radius of half the smallest side.
        /// If a corner does not use up half the smallest side, then the additional space 
        /// is available for the other corner to use if required.
        /// </summary> 
        private void CalculatePath()
        {
            CGRect boundary = this.Bounds;
            var x = boundary.X;
            var y = boundary.Y;
            var w = boundary.Width;
            var h = boundary.Height;

            // check to see if this needs to be square or circle
            if (ForcedBoxShape != ForcedBoxShape.Default)
            {
                var rad = (nfloat)Math.Min(w, h);
                if (w > h)
                {
                    x = x + ((w - rad) / 2);
                }
                else if (h > w)
                {
                    y = y + ((h - rad) / 2);
                }
                w = h = rad;
                if (ForcedBoxShape == ForcedBoxShape.Circle)
                {
                    // if its a circle, force all the corner radii to be equal to half the width
                    CornerRadius = new CornerRadius(rad / 2);
                }
            }


            var tlr = (nfloat)CornerRadius.TopLeft;
            var trr = (nfloat)CornerRadius.TopRight;
            var brr = (nfloat)CornerRadius.BottomRight;
            var blr = (nfloat)CornerRadius.BottomLeft;

            nfloat tlcs, tlce, trcs, trce, brcs, brce, blcs, blce = 0;

            // Calculate the ideal endpoints for each corner. These are distances
            // from the rectangles corner.
            // Top left start and Bottom left end
            GetCurveEnds(blr, tlr, h, out blce, out tlcs);
            // Top left end and Top right start
            GetCurveEnds(tlr, trr, w, out tlce, out trcs);
            // Top right end and Bottom right start
            GetCurveEnds(tlr, brr, h, out trce, out brcs);
            // Bottom right end and Bottom left start
            GetCurveEnds(brr, blr, w, out brce, out blcs);

            // Adjust the corner radii to reflect the newly calculated values
            // Corners will be uniform (not currently supporting ellipsoid corners)
            // so the start and end point distances for each corner must be the same.
            tlr = NMath.Min(tlcs, tlce);
            trr = NMath.Min(trcs, trce);
            brr = NMath.Min(brcs, brce);
            blr = NMath.Min(blcs, blce);


            // Create the path as a Bezier path
            UIBezierPath polygonPath = new UIBezierPath();
            polygonPath.MoveTo(new CGPoint(x, y + tlr));
            polygonPath.AddArc(new CGPoint(x + tlr, y + tlr), tlr, NMath.PI, 3 * NMath.PI / 2, true);
            polygonPath.AddLineTo(new CGPoint(x + w - trr, y));
            polygonPath.AddArc(new CGPoint(x + w - trr, y + trr), trr, 3 * NMath.PI / 2, 0, true);              
            polygonPath.AddLineTo(new CGPoint(x + w, y + h - brr));
            polygonPath.AddArc(new CGPoint(x + w - brr, y + h - brr), brr, 0, NMath.PI / 2, true);
            polygonPath.AddLineTo(new CGPoint(x + blr, y + h));
            polygonPath.AddArc(new CGPoint(x + blr, y + h - blr), blr, NMath.PI / 2, NMath.PI, true);
            polygonPath.AddLineTo(new CGPoint(x, y + tlr));
            polygonPath.ClosePath();
            // Convert to a CGPath
            shapePath = polygonPath.CGPath;
        }

        /// <summary>
        /// This implements the algorithm to calculate the ideal distances of the curve
        /// ends from the rectangle corner.
        /// If two corners would overlap, their radii are adjusted to prevent this.
        /// Each corner is guaranteed a radius of half the smallest side.
        /// If a corner does not use up half the smallest side, then the additional space 
        /// is available for the other corner to use if required.
        /// </summary>
        /// <param name="rad1">Radius of first corner</param>
        /// <param name="rad2">Radius of second corner</param>
        /// <param name="edgeLength">Length of the rectangle edge joining the corners</param>
        /// <param name="d1">Distance of the corner end of the first corner from the associated rectangle corner</param>
        /// <param name="d2">Distance of the corner end of the second corner from the associated rectangle corner</param>
        private void GetCurveEnds(nfloat rad1, nfloat rad2, nfloat edgeLength, out nfloat d1, out nfloat d2)
        {
            d1 = d2 = 0;
            if (rad1 + rad2 <= edgeLength)
            {
                d1 = rad1;
                d2 = rad2;
            }
            else
            {
                var mid = edgeLength / 2;
                if (rad1 > mid && rad2 > mid)
                {
                    d1 = mid;
                    d2 = mid;
                }
                else if (rad1 <= mid)
                {
                    d1 = rad1;
                    d2 = edgeLength - d1;
                }
                else
                {
                    d2 = rad2;
                    d1 = edgeLength - d2;
                }
            }
        }



        /// <summary> 
        /// Updates the layer's mask. This is required as the view will be drawn in rectangle
        /// which needs to be masked out
        /// </summary> 
        private void UpdateMask()
        {
            if (shapePath == null)
            {
                CalculatePath();
            }
            CAShapeLayer maskLayer = new CAShapeLayer();
            maskLayer.Frame = this.Bounds;
            maskLayer.Path = shapePath;
            // Set the newly created shape layer as the mask for the image view's layer 
            this.Layer.Mask = maskLayer;
        }


        /// <summary> 
        /// Override the Draw method to draw the require shape path on the current
        /// UIGrpahicsContext (canvas), setting the fill color, border thickness and 
        /// border color to be used on that context.
        /// </summary> 
        public override void Draw(CGRect rect)
        {
            Update();
            using (CGContext g = UIGraphics.GetCurrentContext())
            {
                //set up drawing attributes
                g.SetLineWidth((float)BorderThickness);
                this.BackgroundColor.SetFill();
                this.BorderColor.SetStroke();
                //add geometry to graphics context and draw it
                g.AddPath(shapePath);
                // g.Clip(); // this doesn't seem to work correctly, hence the need to use mask
                g.DrawPath(CGPathDrawingMode.FillStroke);
            }
        }

    }



}