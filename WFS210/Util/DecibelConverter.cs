using System;

namespace WFS210
{
	public static class DecibelConverter
	{
		public static string ToString(double voltage, int precision = 2)
		{
			return UnitConverter.ToString (voltage, new string[] {"dBm"}, 1, precision);
		}
	}
}

