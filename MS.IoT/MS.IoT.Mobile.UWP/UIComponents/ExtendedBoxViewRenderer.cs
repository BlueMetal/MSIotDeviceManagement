using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

using Xamarin.Forms;
using Xamarin.Forms.Platform.UWP;

using UWPColor = Windows.UI.Color;
using UWPGrid = Windows.UI.Xaml.Controls.Grid;
using UWPSize = Windows.Foundation.Size;
using UWPThickness = Windows.UI.Xaml.Thickness;
using XFColor = Xamarin.Forms.Color;

using QXFUtilities;
using QXFUtilities.Controls;
using QXFUtilities.UWP;



[assembly: ExportRenderer(typeof(QXFUtilities.Controls.ExtendedBoxView), typeof(QXFUtilities.Controls.UWP.ExtendedBoxViewRenderer))]
namespace QXFUtilities.Controls.UWP
{

    public class ExtendedBoxViewRenderer : ViewRenderer<Xamarin.Forms.BoxView, Windows.UI.Xaml.Controls.Grid>
    {

        private UWPSize? originalCanvasSize = null;
        private CornerRadius? savedCornerRadius = null;

        public ExtendedBoxViewRenderer()
        {
            SizeChanged += OnSizeChanged;
        }

        private void OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (!e.NewSize.IsEmpty)
            {
                ExtendedBoxView ebv = (ExtendedBoxView)this.Element;
                if (originalCanvasSize == null)
                {
                    originalCanvasSize = e.NewSize;
                }

                if (ebv.ForcedBoxShape != ForcedBoxShape.Default)
                {
                    var rad = Math.Min(e.NewSize.Width, e.NewSize.Height);


                    if (ebv.ForcedBoxShape == ForcedBoxShape.Circle)
                    {
                        if (savedCornerRadius == null)
                        {
                            savedCornerRadius = ebv.CornerRadius;
                        }
                        ebv.CornerRadius = new CornerRadius(rad / 2);
                    }   
                    UWPGrid grid = CreateExtendedBox();                                
                    grid.Height = rad;
                    grid.Width = rad;
                    SetNativeControl(grid);
                }
            }
        }

        protected override void OnElementChanged(ElementChangedEventArgs<BoxView> e)
        {
            base.OnElementChanged(e);

            // Set up the 'parent' Native Element that will be used to contain all the elements in the Native Control
            // This is the native control that will be assigned
            // Note: The windows layout system and the forms layout system interact to automatically layout 
            // the grid according to the LayoutOptions specified in the Forms control

            UWPGrid grid = CreateExtendedBox();
            // set the grid to be the native control
            SetNativeControl(grid);
        }


        private UWPGrid CreateExtendedBox()
        {

            UWPGrid grid = new UWPGrid();

            ExtendedBoxView ebv = (ExtendedBoxView)this.Element;

            if (ebv.IsVisible)
            {

                // Convert the parameters in the Forms Element so they can be used in the Native Control

                XFColor rectClr = ebv.Color;
                UWPColor rectColour = UWPColor.FromArgb((byte)(255 * (rectClr.A < 0f ? 0 : rectClr.A)),
                                                     (byte)(255 * (rectClr.R < 0f ? 0 : rectClr.R)),
                                                     (byte)(255 * (rectClr.G < 0f ? 0 : rectClr.G)),
                                                     (byte)(255 * (rectClr.B < 0f ? 0 : rectClr.B)));

                XFColor brdClr = ebv.BorderColor;
                UWPColor brdColour = UWPColor.FromArgb((byte)(255 * (brdClr.A < 0f ? 0 : brdClr.A)),
                                                    (byte)(255 * (brdClr.R < 0f ? 0 : brdClr.R)),
                                                    (byte)(255 * (brdClr.G < 0f ? 0 : brdClr.G)),
                                                    (byte)(255 * (brdClr.B < 0f ? 0 : brdClr.B)));



                // Set up a rectangle representing the Box 
                // Note1: Don't need to specify the Layout options or size of the Rectangle, as it will fill the cell contained
                //        in the grid. So the cell and grid layout will dictate the size and layout behaviour of the rectangle.
                // Note2: (Setting the layout and/or size of the rectangle will override those of the cell/grid. This may
                //         cause some of the behaviour to be different than expected, even if manually picking up the values
                //         from the Forms control)

                Border rect = new Border
                {
                    Background = new SolidColorBrush(rectColour),
                    BorderBrush = new SolidColorBrush(brdColour),
                    BorderThickness = new UWPThickness(ebv.BorderThickness),
                    CornerRadius = CornerRadiusConverter.Convert(ebv.CornerRadius)
                };

                // Set up a rectangle representing the Shadow
                if (ebv.HasShadow)
                {
                    Border shadow = CreateShadow(ebv.ShadowColor, ebv.CornerRadius, ebv.BorderThickness,
                                                 ebv.ShadowPosition.dX, ebv.ShadowPosition.dY, ebv.Color.A, ebv.BorderColor.A);
                    grid.Children.Add(shadow);
                }

                // Add the box rectangle to the grid
                // Note:  The default behaviour for a grid with no column definitions or row definitions is a single cell
                //        filling the available space in the grid. The windows layout system and the forms layout system
                //        interact to automatically layout cell according to the LayoutOptions and Size Requests 
                //        specified in the Forms control 
                grid.Children.Add(rect);
            }

            return grid;

        }



        protected override void OnElementPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);


            if (e.PropertyName == ExtendedBoxView.ColorProperty.PropertyName)
            {
                XFColor clr = ((ExtendedBoxView)this.Element).Color;
                UWPColor wColour = UWPColor.FromArgb((byte)(255 * (clr.A < 0f ? 0 : clr.A)),
                                                  (byte)(255 * (clr.R < 0f ? 0 : clr.R)),
                                                  (byte)(255 * (clr.G < 0f ? 0 : clr.G)),
                                                  (byte)(255 * (clr.B < 0f ? 0 : clr.B)));
                ((Border)this.Control.Children[this.Control.Children.Count - 1]).Background = new SolidColorBrush(wColour);
            }
            else if (e.PropertyName == ExtendedBoxView.CornerRadiusProperty.PropertyName)
            {
                ExtendedBoxView ebv = (ExtendedBoxView)this.Element;
                int mainRectIndex = this.Control.Children.Count - 1;
                ((Border)this.Control.Children[mainRectIndex]).CornerRadius = CornerRadiusConverter.Convert(ebv.CornerRadius);
                if (mainRectIndex > 0) // grid has a shadow rectangle in it
                {
                    ((Border)this.Control.Children[mainRectIndex - 1]).CornerRadius = CornerRadiusConverter.Convert(ebv.CornerRadius);
                }
            }
            else if (e.PropertyName == ExtendedBoxView.BorderColorProperty.PropertyName)
            {
                XFColor clr = ((ExtendedBoxView)this.Element).BorderColor;
                UWPColor wColour = UWPColor.FromArgb((byte)(255 * (clr.A < 0f ? 0 : clr.A)),
                                                  (byte)(255 * (clr.R < 0f ? 0 : clr.R)),
                                                  (byte)(255 * (clr.G < 0f ? 0 : clr.G)),
                                                  (byte)(255 * (clr.B < 0f ? 0 : clr.B)));
                ((Border)this.Control.Children[this.Control.Children.Count - 1]).BorderBrush = new SolidColorBrush(wColour);
            }
            else if (e.PropertyName == ExtendedBoxView.BorderThicknessProperty.PropertyName)
            {
                ExtendedBoxView ebv = (ExtendedBoxView)this.Element;
                int mainRectIndex = this.Control.Children.Count - 1;
                ((Border)this.Control.Children[mainRectIndex]).BorderThickness = new UWPThickness(ebv.BorderThickness);
                if (mainRectIndex > 0) // grid has a shadow rectangle in it
                {
                    ((Border)this.Control.Children[mainRectIndex - 1]).BorderThickness = new UWPThickness(ebv.BorderThickness);
                }
            }
            else if (e.PropertyName == ExtendedBoxView.HasShadowProperty.PropertyName)
            {
                ExtendedBoxView ebv = (ExtendedBoxView)this.Element;
                if (ebv.HasShadow)
                {
                    // Create the shadow
                    Border shadow = CreateShadow(ebv.ShadowColor, ebv.CornerRadius, ebv.BorderThickness,
                                                 ebv.ShadowPosition.dX, ebv.ShadowPosition.dY, ebv.Color.A, ebv.BorderColor.A);
                    // check in case the grid already contains a shadow rectangle
                    if (this.Control.Children.Count < 2)
                    {
                        this.Control.Children.Insert(0, shadow);
                    }
                }
                else
                {
                    // check that the shadow is in the grid
                    if (this.Control.Children.Count > 1)
                    {
                        // remove the shadow - first element in grid
                        this.Control.Children.RemoveAt(0);
                    }
                }
            }
            else if (e.PropertyName == ExtendedBoxView.ShadowColorProperty.PropertyName)
            {
                ExtendedBoxView ebv = (ExtendedBoxView)this.Element;
                if (ebv.HasShadow)
                {
                    int mainRectIndex = this.Control.Children.Count - 1;
                    // check there is a shadow to change
                    if (mainRectIndex > 0)
                    {
                        Xamarin.Forms.Color clr = ebv.ShadowColor;
                        UWPColor sColour = UWPColor.FromArgb((byte)(255 * (clr.A < 0f ? 0 : clr.A)),
                                                          (byte)(255 * (clr.R < 0f ? 0 : clr.R)),
                                                          (byte)(255 * (clr.G < 0f ? 0 : clr.G)),
                                                          (byte)(255 * (clr.B < 0f ? 0 : clr.B)));
                        ((Border)this.Control.Children[mainRectIndex - 1]).Background = new SolidColorBrush(sColour);
                        ((Border)this.Control.Children[mainRectIndex - 1]).BorderBrush = new SolidColorBrush(sColour);
                    }
                }
            }
            else if (e.PropertyName == ExtendedBoxView.ShadowPositionProperty.PropertyName)
            {
                ExtendedBoxView ebv = (ExtendedBoxView)this.Element;
                if (ebv.HasShadow)
                {
                    int mainRectIndex = this.Control.Children.Count - 1;
                    // check there is a shadow to change
                    if (mainRectIndex > 0)
                    {
                        ((Border)this.Control.Children[mainRectIndex - 1]).RenderTransform =
                            new CompositeTransform { TranslateX = ebv.ShadowPosition.dX, TranslateY = ebv.ShadowPosition.dY };
                    }
                }
            }
            else if (e.PropertyName == BoxView.IsVisibleProperty.PropertyName)
            {
 //               UWPGrid grid = CreateExtendedBox();
//                SetNativeControl(grid);
                ExtendedBoxView ebv = (ExtendedBoxView)this.Element;
                int mainRectIndex = this.Control.Children.Count - 1;
                Visibility visibility = Visibility.Visible;
                if (!ebv.IsVisible)
                {
                    visibility = Visibility.Collapsed;
                }

                ((Border)this.Control.Children[mainRectIndex]).Visibility = visibility;
                if (mainRectIndex > 0) // grid has a shadow rectangle in it
                {
                    ((Border)this.Control.Children[mainRectIndex - 1]).Visibility = visibility;
                }

                /*
                // remove all children from the grid
                for (int i = this.Control.Children.Count - 1; i >= 0; i--)
                {
                    this.Control.Children.RemoveAt(i);
                }
                // Recreate
                CreateExtendedBox(this.Control);
                this.Control.UpdateLayout();
                */
                /*
                if (this.Element.IsVisible)
                {
                    this.Control.Visibility = Windows.UI.Xaml.Visibility.Visible;
                }
                else
                {
                    this.Control.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                }
                */
            }
            else if (e.PropertyName == ExtendedBoxView.ForcedBoxShapeProperty.PropertyName)
            {
                UWPGrid grid = new UWPGrid();         
                ExtendedBoxView ebv = (ExtendedBoxView)this.Element;
                var originalCanvasWidth = ((UWPSize)originalCanvasSize).Width;
                var originalCanvasHeight = ((UWPSize)originalCanvasSize).Height;
                
                if ((this.Control.ActualHeight == originalCanvasHeight) && (this.Control.ActualWidth == originalCanvasWidth))
                {
                    // control is currently the original size
                    if (ebv.ForcedBoxShape == ForcedBoxShape.Default)
                    {
                        // Should never get here because already fitted to the original size
                        if (savedCornerRadius != null)
                        {
                            ebv.CornerRadius = (CornerRadius)savedCornerRadius;
                            savedCornerRadius = null;
                        }
                        grid = CreateExtendedBox();
                    }
                    else
                    {
                        var rad = Math.Min(originalCanvasWidth, originalCanvasHeight);
                        if (ebv.ForcedBoxShape == ForcedBoxShape.Circle)
                        {
                            savedCornerRadius = ebv.CornerRadius;
                            ebv.CornerRadius = new CornerRadius(rad / 2);
                        }
                        else
                        {
                            if (savedCornerRadius != null)
                            {
                                ebv.CornerRadius = (CornerRadius)savedCornerRadius;
                                savedCornerRadius = null;
                            }
                        }
                        grid = CreateExtendedBox();
                        grid.Height = rad;
                        grid.Width = rad;
                    }
                }
                else
                {
                    // control size has been modified
                    if (ebv.ForcedBoxShape == ForcedBoxShape.Default)
                    {
                        // force back to original size
                        if (savedCornerRadius != null)
                        {
                            ebv.CornerRadius = (CornerRadius)savedCornerRadius;
                            savedCornerRadius = null;
                        }
                        grid = CreateExtendedBox();
                    }
                    else
                    {
                        var rad = Math.Min(originalCanvasWidth, originalCanvasHeight);
                        if (ebv.ForcedBoxShape == ForcedBoxShape.Circle)
                        {
                            savedCornerRadius = ebv.CornerRadius;
                            ebv.CornerRadius = new CornerRadius(rad / 2);
                        }
                        else
                        {
                            if (savedCornerRadius != null)
                            {
                                ebv.CornerRadius = (CornerRadius)savedCornerRadius;
                                savedCornerRadius = null;
                            }
                        }
                        grid = CreateExtendedBox();
                        grid.Height = rad;
                        grid.Width = rad;
                    }


                }
                SetNativeControl(grid);
            }

        }


        /// <summary>
        /// This method returns a Border representing the shadow of the box
        /// </summary>
        /// <param name="shadowColour">Colour of the shadow</param>
        /// <param name="cornerRadius">Corner radius of the box</param>
        /// <param name="boxBorderThickness">Thickness of the box border</param>
        /// <param name="xDeltaPosition">X Translation of the shadow from the box</param>
        /// <param name="yDeltaPosition">Y Translation of the shadow from the box</param>
        /// <param name="boxColourAlpha">Alpha (opacity) of the box colour</param>
        /// <param name="boxBorderAlpha">Alpha (opacity) of the box border colour</param>
        /// <returns>Border</returns>
        private Border CreateShadow(XFColor shadowColour, CornerRadius cornerRadius, double boxBorderThickness,
                                        double xDeltaPosition, double yDeltaPosition, double boxColourAlpha, double boxBorderAlpha)
        {
            // Create a rectangle representing the shadow
 
            //Adjust the shadow color to account for any transparency in the box.
            XFColor clr = new XFColor(shadowColour.R, shadowColour.G, shadowColour.B, shadowColour.A * boxColourAlpha);
            XFColor borderClr = new XFColor(shadowColour.R, shadowColour.G, shadowColour.B, shadowColour.A * boxBorderAlpha);

            UWPColor shdwColour = UWPColor.FromArgb((byte)(255 * (clr.A < 0f ? 0 : clr.A)),
                                              (byte)(255 * (clr.R < 0f ? 0 : clr.R)),
                                              (byte)(255 * (clr.G < 0f ? 0 : clr.G)),
                                              (byte)(255 * (clr.B < 0f ? 0 : clr.B)));
            UWPColor shadowBorderColour = UWPColor.FromArgb((byte)(255 * (borderClr.A < 0f ? 0 : borderClr.A)),
                                              (byte)(255 * (borderClr.R < 0f ? 0 : borderClr.R)),
                                              (byte)(255 * (borderClr.G < 0f ? 0 : borderClr.G)),
                                              (byte)(255 * (borderClr.B < 0f ? 0 : borderClr.B)));

            return  new Border
            {
                Background = new SolidColorBrush(shdwColour),
                BorderBrush = new SolidColorBrush(shadowBorderColour),
                BorderThickness = new UWPThickness(boxBorderThickness),
                CornerRadius = CornerRadiusConverter.Convert(cornerRadius),
                RenderTransform = new CompositeTransform { TranslateX = xDeltaPosition, TranslateY = yDeltaPosition }
            };
        }

    }




}
