using System;
using Xamarin.Forms;
using QXFUtilities;

namespace QXFUtilities.Controls
{    

    public class ExtendedBoxView : BoxView
    {


 
#region New Bindable Properties   
        // New Bindable Properties             
        public static readonly BindableProperty CornerRadiusProperty = BindableProperty.Create
            (nameof(CornerRadius), typeof(CornerRadius), typeof(ExtendedBoxView), new CornerRadius(0.0));
        public CornerRadius CornerRadius
        {
            get { return (CornerRadius)GetValue(CornerRadiusProperty); }  // from BindableObject base class
            set { SetValue(CornerRadiusProperty, value); }          // from BindableObject base class
        }

        public static readonly BindableProperty BorderThicknessProperty = BindableProperty.Create
            (nameof(BorderThickness), typeof(double), typeof(ExtendedBoxView), 0.0);
        public double BorderThickness
        {
            get { return (double)GetValue(BorderThicknessProperty); }
            set { SetValue(BorderThicknessProperty, value); }
        }




        public static readonly BindableProperty BorderColorProperty = BindableProperty.Create
            (nameof(BorderColor), typeof(Color), typeof(ExtendedBoxView), Color.Transparent);
        public Color BorderColor
        {
            get { return (Color)GetValue(BorderColorProperty); }
            set { SetValue(BorderColorProperty, value); }
        }


        public static readonly BindableProperty HasShadowProperty = BindableProperty.Create
            (nameof(HasShadow), typeof(bool), typeof(ExtendedBoxView), false);
        public bool HasShadow
        {
            get { return (bool)GetValue(HasShadowProperty); }
            set { SetValue(HasShadowProperty, value); }
        }

        

        public static readonly BindableProperty ShadowPositionProperty = BindableProperty.Create
            (nameof(ShadowPosition), typeof(Translation2D), typeof(ExtendedBoxView), new Translation2D(4.0));
        public Translation2D ShadowPosition
        {
            get { return (Translation2D)GetValue(ShadowPositionProperty); }
            set { SetValue(ShadowPositionProperty, value); }
        }

        

        public static readonly BindableProperty ShadowColorProperty = BindableProperty.Create
          (nameof(ShadowColor), typeof(Color), typeof(ExtendedBoxView), Color.Gray);
        public Color ShadowColor
        {
            get { return (Color)GetValue(ShadowColorProperty); }
            set { SetValue(ShadowColorProperty, value); }
        }



        public static readonly BindableProperty Android_ElevationProperty = BindableProperty.Create
            (nameof(Android_Elevation), typeof(double), typeof(ExtendedBoxView), 0.0);
        public double Android_Elevation
        {
            get { return (double)GetValue(Android_ElevationProperty); }
            set { SetValue(Android_ElevationProperty, value); }
        }



        public static readonly BindableProperty ForcedBoxShapeProperty = BindableProperty.Create
            (nameof(ForcedBoxShape), typeof(ForcedBoxShape), typeof(ExtendedBoxView), ForcedBoxShape.Default);
        public ForcedBoxShape ForcedBoxShape
        {
            get { return (ForcedBoxShape)GetValue(ForcedBoxShapeProperty); }
            set { SetValue(ForcedBoxShapeProperty, value); }
        }
#endregion



    }

    public enum ForcedBoxShape
    {
        Default,
        Square,
        Circle
    }


    public class ForcedBoxShapeTypeConverter : TypeConverter
    {
        public override object ConvertFromInvariantString(string value)
        {
            if (value != null)
            {
                switch (value)
                {
                    case "Square":
                        return ForcedBoxShape.Square;
                    case "Circle":
                        return ForcedBoxShape.Circle;
                    default:
                        return ForcedBoxShape.Default;
                }
            }

            throw new InvalidOperationException(string.Format("Cannot convert \"{0}\" into {1}", value, typeof(ForcedBoxShape)));
        }


    }


}
