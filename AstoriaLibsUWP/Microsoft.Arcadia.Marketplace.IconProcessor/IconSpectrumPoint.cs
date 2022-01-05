using Microsoft.Arcadia.Marketplace.IconProcessor.Imaging;

namespace Microsoft.Arcadia.Marketplace.IconProcessor
{
	internal class IconSpectrumPoint
	{
		internal Color Color { get; set; }

		internal int Margin { get; set; }

		internal int MaxDistanceFromBg { get; set; }

		public IconSpectrumPoint(int margin, Color color)
		{
			Color = color;
			Margin = margin;
			MaxDistanceFromBg = 0;
		}
	}
}