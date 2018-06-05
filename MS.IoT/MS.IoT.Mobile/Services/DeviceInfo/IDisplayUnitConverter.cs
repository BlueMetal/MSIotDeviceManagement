using System;

namespace QXFUtilities.DeviceInfo
{
	public interface IDisplayUnitConverter
	{
		float ScalingFactor { get;}
		DisplayCatagory DisplayCatagory { get;}

		float Convert(float value, DisplayUnit from, DisplayUnit to);
	}
}

