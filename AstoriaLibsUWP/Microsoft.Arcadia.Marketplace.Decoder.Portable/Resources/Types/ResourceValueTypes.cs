using System;

namespace Microsoft.Arcadia.Marketplace.Decoder.Portable.Resources.Types
{
	[Flags]
	public enum ResourceValueTypes
	{
		None = 0,
		Reference = 1,
		Attribute = 2,
		String = 3,
		Float = 4,
		Dimension = 5,
		Fraction = 6,
		FirstInt = 16, //0x10
		IntDec = 16, //0x10
		IntHex = 17, //0x11
		IntBoolean = 18, //0x12
		FirstColorInt = 28, //0x1C
		IntColorArgb8 = 28, //0x1C
		IntColorRgb8 = 29, //0x1D
		IntColorArgb4 = 30, //0x1E
		IntColorRgb4 = 31, //0x1F
		LastColorInt = 31, //0x1F
		LastInt = 31 //0x1F
	}
}