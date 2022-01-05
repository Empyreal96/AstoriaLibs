using Microsoft.Arcadia.Marketplace.IconProcessor.Imaging;

namespace Microsoft.Arcadia.Marketplace.IconProcessor
{
	public static class ColorConstants
	{
		public static readonly Color White = Color.FromArgb(byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue);

		public static readonly Color Red = Color.FromArgb(byte.MaxValue, byte.MaxValue, 0, 0);

		public static readonly Color DarkBlue = Color.FromArgb(byte.MaxValue, 58, 1, 231);

		public static readonly Color DarkGray = Color.FromArgb(byte.MaxValue, 120, 118, 112);

		public static readonly Color Green = Color.FromArgb(byte.MaxValue, 73, 201, 31);
	}
}