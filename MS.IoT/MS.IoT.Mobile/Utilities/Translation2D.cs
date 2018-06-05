using Xamarin.Forms;

namespace QXFUtilities
{
    // a struct to hold two values that represent a translation along the x and y axis
    // Would use the XF Vec2 struct, however is does not have a Type Converter or other methods 


    [TypeConverter(typeof(Translation2DTypeConverter))]
    public struct Translation2D
    {
        public double dX { get; set; }
        public double dY { get; set; }


        public Translation2D(double uniformTranslation) : this(uniformTranslation, uniformTranslation)
        {
        }

        public Translation2D(double dx, double dy) : this()
        {
            this.dX = dx;
            this.dY = dy;
        }


        public static implicit operator Translation2D(double uniformTranslation)
        {
            return new Translation2D(uniformTranslation);
        }


        bool Equals(Translation2D other)
        {
            return dX.Equals(other.dX) && dY.Equals(other.dY);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
                return false;
            return obj is Translation2D && Equals((Translation2D)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = dX.GetHashCode();
                hashCode = (hashCode * 397) ^ dY.GetHashCode();
                return hashCode;
            }
        }


        public static bool operator ==(Translation2D translation1, Translation2D translation2)
        {
            return translation1.Equals(translation2);
        }

        public static bool operator !=(Translation2D translation1, Translation2D translation2)
        {
            return !translation1.Equals(translation2);
        }

        public bool EqualsZero()
        {
            return (dX == 0) && (dY == 0);
        }



    }

}
