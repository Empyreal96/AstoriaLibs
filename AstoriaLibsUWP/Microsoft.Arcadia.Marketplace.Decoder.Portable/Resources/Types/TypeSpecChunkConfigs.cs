using System;

namespace Microsoft.Arcadia.Marketplace.Decoder.Portable.Resources.Types
{
	[Flags]
	public enum TypeSpecChunkConfigs
	{
		Mcc = 1,
		Mnc = 2,
		Locale = 4,
		Touchscreen = 8,
		Keyboard = 16, //0x10
		KeyboardHidden = 32, //0x20
		Navigation = 64, //0x40
		Orientation = 128, //0x80
		Density = 256, //0x100
		ScreenSize = 512, //0x200
		Version = 1024, //0x400
		ScreenLayout = 2048, //0x800
		UiMode = 4096, //0x1000
		SmallestScreenSize = 8192, //0x2000
		LayoutDir = 16384, //0x4000
		NotYetUsed = 0x8000,
		MncZero = 0xFFFF
	}
}