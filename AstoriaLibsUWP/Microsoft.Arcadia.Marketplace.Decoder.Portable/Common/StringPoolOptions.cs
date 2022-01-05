using System;

namespace Microsoft.Arcadia.Marketplace.Decoder.Portable.Common
{
	[Flags]
	public enum StringPoolOptions
	{
		Sorted = 1,
		Utf8 = 0x100
	}
}