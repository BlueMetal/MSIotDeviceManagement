using System;
using System.Globalization;

using Xamarin.Forms;

namespace QXFUtilities
{
    public class CornerRadiusTypeConverter : TypeConverter
    {
        public override object ConvertFromInvariantString(string value)
 		{ 
 			if (value != null) 
 			{ 
 				double tl, tr, br, bl; 
 				string[] radius = value.Split(','); 
 				switch (radius.Length) 
 				{ 
 					case 1: 
 						if (double.TryParse(radius[0], NumberStyles.Number, CultureInfo.InvariantCulture, out tl)) 
 							return new CornerRadius(tl); 
 						break;  
 					case 4: 
 						if (double.TryParse(radius[0], NumberStyles.Number, CultureInfo.InvariantCulture, out tl) && double.TryParse(radius[1], NumberStyles.Number, CultureInfo.InvariantCulture, out tr) && 
 							double.TryParse(radius[2], NumberStyles.Number, CultureInfo.InvariantCulture, out br) && double.TryParse(radius[3], NumberStyles.Number, CultureInfo.InvariantCulture, out bl)) 
 							return new CornerRadius(tl, tr, br, bl); 
 						break; 
 				} 
 			} 
 
 			throw new InvalidOperationException(string.Format("Cannot convert \"{0}\" into {1}", value, typeof(CornerRadius))); 
 		} 


    }
}
