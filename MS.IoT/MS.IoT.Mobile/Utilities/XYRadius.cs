namespace QXFUtilities
{
    public struct XYRadius
    {
        public double XRadius;
        public double YRadius;


        public XYRadius(double uniformRadius)
        {
            XRadius = uniformRadius;
            YRadius = uniformRadius;
        }

        public XYRadius(double xRadius, double yRadius)
        {
            XRadius = xRadius;
            YRadius = yRadius;
        }

    }
}
