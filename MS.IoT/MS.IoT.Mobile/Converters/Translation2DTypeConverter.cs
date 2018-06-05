using System;
using System.Globalization;

using Xamarin.Forms;

namespace QXFUtilities
{
    public class Translation2DTypeConverter : TypeConverter
    {
        public override object ConvertFromInvariantString(string value)
        {
            if (value != null)
            {
                double dx, dy;
                string[] deltas = value.Split(',');
                switch (deltas.Length)
                {
                    case 1:
                        if (double.TryParse(deltas[0], NumberStyles.Number, CultureInfo.InvariantCulture, out dx))
                            return new Translation2D(dx);
                        break;
                    case 2:
                        if (double.TryParse(deltas[0], NumberStyles.Number, CultureInfo.InvariantCulture, out dx) && double.TryParse(deltas[1], NumberStyles.Number, CultureInfo.InvariantCulture, out dy))
                            return new Translation2D(dx, dy);
                        break;
                }
            }

            throw new InvalidOperationException(string.Format("Cannot convert \"{0}\" into {1}", value, typeof(Translation2D)));
        }
    }
}
