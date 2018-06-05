using Xamarin.Forms;

namespace QXFUtilities
{
    [TypeConverter(typeof(CornerRadiusTypeConverter))]
    public struct CornerRadius
    {
        public double TopLeft { get; set; }
        public double TopRight { get; set; }
        public double BottomRight { get; set; }
        public double BottomLeft { get; set; }


        public CornerRadius(double uniformRadius) : this(uniformRadius, uniformRadius, uniformRadius, uniformRadius)
        {
        }

        public CornerRadius(double topLeft, double topRight, double bottomRight, double bottomLeft) : this()
        {
            this.TopLeft = topLeft;
            this.TopRight = topRight;
            this.BottomRight = bottomRight;
            this.BottomLeft = bottomLeft;
        }


        public static implicit operator CornerRadius(double uniformRadius)
 		{ 
 			return new CornerRadius(uniformRadius); 
 		} 
 
 
 		bool Equals(CornerRadius other)
 		{ 
 			return TopLeft.Equals(other.TopLeft) && TopRight.Equals(other.TopRight) && BottomRight.Equals(other.BottomRight) && BottomLeft.Equals(other.BottomLeft); 
 		}

        public override bool Equals(object obj)
 		{ 
 			if (ReferenceEquals(null, obj)) 
 				return false; 
 			return obj is CornerRadius && Equals((CornerRadius)obj); 
 		}

        public override int GetHashCode()
 		{ 
 			unchecked 
 			{ 
 				int hashCode = TopLeft.GetHashCode(); 
 				hashCode = (hashCode* 397) ^ TopRight.GetHashCode(); 
 				hashCode = (hashCode* 397) ^ BottomRight.GetHashCode(); 
 				hashCode = (hashCode* 397) ^ BottomLeft.GetHashCode(); 
 				return hashCode; 
 			} 
 		} 
 
 
 		public static bool operator == (CornerRadius radius1, CornerRadius radius2)
 		{ 
 			return radius1.Equals(radius2); 
 		} 
 
  		public static bool operator != (CornerRadius radius1, CornerRadius radius2)
 		{ 
 			return !radius1.Equals(radius2); 
 		} 

        public bool EqualsZero()
        {
            return (TopLeft == 0) && (TopRight == 0) && (BottomRight == 0) && (BottomLeft == 0);
        }

        public bool HasUniformCorners()
        {
            return (TopLeft == TopRight) && (TopRight == BottomRight) && (BottomRight == BottomLeft);
        }

        public bool HasUniformOrZeroCorners()
        {
            bool result = true;
            double? val = null;
            if (TopLeft != 0)
            {
                val = TopLeft;
            }
            if (TopRight != 0)
            {
                if (val == null)
                {
                    val = TopRight;
                }
                else
                {
                    result = TopRight == val;
                }
            }
            if (result)
            {
                if (BottomRight != 0)
                {
                    if (val == null)
                    {
                        val = BottomRight;
                    }
                    else
                    {
                        result = BottomRight == val;
                    }
                }
            }
            if (result)
            {
                if (BottomLeft != 0)
                {
                    if (val != null)
                    {
                        result = BottomLeft == val;
                    }
                }
            }

            return result;
        }


    }
}
