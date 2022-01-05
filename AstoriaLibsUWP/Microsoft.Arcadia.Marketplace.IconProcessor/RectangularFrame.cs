using Microsoft.Arcadia.Marketplace.IconProcessor.Imaging;

namespace Microsoft.Arcadia.Marketplace.IconProcessor
{
	public class RectangularFrame
	{
		internal delegate bool EnumDelegate(int x, int y);

		internal int Width { get; set; }

		internal int Height { get; set; }

		internal int Margin { get; set; }

		internal int CornerRadius { get; set; }

		internal RectangularFrameSide HorizontalSide { get; private set; }

		internal RectangularFrameSide VerticalSide { get; private set; }

		internal static void ExtendTheFrame(Image bitmap, int horizontalMargin, int verticalMargin)
		{
			for (int i = 0; i < bitmap.Height; i++)
			{
				ExtendOneHorizontalLineOfFrames(bitmap, horizontalMargin, i);
			}
			for (int j = 0; j < bitmap.Width; j++)
			{
				ExtendOneVerticalLineOfFrames(bitmap, verticalMargin, j);
			}
		}

		internal void ForEachPixel(EnumDelegate del)
		{
			InitializeSides();
			for (int i = VerticalSide.EdgeTestStart; i < VerticalSide.EdgeTestEnd; i++)
			{
				int distanceFromTheEdge = VerticalSide.GetDistanceFromTheEdge(i);
				int x = Width - VerticalSide.GetDistanceFromTheEdge(i) - 1;
				if (!del(distanceFromTheEdge, i) || !del(x, i))
				{
					return;
				}
			}
			for (int j = HorizontalSide.EdgeTestStart; j < HorizontalSide.EdgeTestEnd; j++)
			{
				int distanceFromTheEdge2 = HorizontalSide.GetDistanceFromTheEdge(j);
				int y = Height - HorizontalSide.GetDistanceFromTheEdge(j) - 1;
				if (!del(j, distanceFromTheEdge2) || !del(j, y))
				{
					break;
				}
			}
		}

		internal void FillMarginsAndCorners(Image graphics, Color brush)
		{
			graphics.FillRectangle(brush, 0, 0, Margin, Height);
			graphics.FillRectangle(brush, Width - Margin, 0, Margin, Height);
			graphics.FillRectangle(brush, 0, Height - Margin, Width, Margin);
			graphics.FillRectangle(brush, 0, 0, Width, Margin);
			int num = Width - 1;
			int num2 = Height - 1;
			int num3 = Margin + CornerRadius;
			for (int i = Margin; i < Margin + num3; i++)
			{
				for (int j = 0; j < VerticalSide.GetDistanceFromTheEdge(i); j++)
				{
					graphics.SetPixel(j, i, brush);
					graphics.SetPixel(num - j, i, brush);
				}
				int y = num2 - i;
				for (int k = 0; k < VerticalSide.GetDistanceFromTheEdge(y); k++)
				{
					graphics.SetPixel(k, y, brush);
					graphics.SetPixel(num - k, y, brush);
				}
			}
		}

		internal void ExtendTheFrameHorizontally(Image bitmap)
		{
			InitializeSides();
			for (int i = 0; i < Height; i++)
			{
				int distanceFromTheEdge = VerticalSide.GetDistanceFromTheEdge(i);
				ExtendOneHorizontalLineOfFrames(bitmap, distanceFromTheEdge, i);
			}
		}

		private static void ExtendOneHorizontalLineOfFrames(Image bitmap, int edgeDistance, int verticalCoordinate)
		{
			Color pixel = bitmap.GetPixel(edgeDistance, verticalCoordinate);
			for (int i = 0; i <= edgeDistance; i++)
			{
				bitmap.SetPixel(i, verticalCoordinate, pixel);
			}
			Color pixel2 = bitmap.GetPixel(bitmap.Width - edgeDistance - 2, verticalCoordinate);
			for (int num = bitmap.Width - 1; num > bitmap.Width - edgeDistance - 2; num--)
			{
				bitmap.SetPixel(num, verticalCoordinate, pixel2);
			}
		}

		private static void ExtendOneVerticalLineOfFrames(Image bitmap, int edgeDistance, int horizontalCoordinate)
		{
			Color pixel = bitmap.GetPixel(horizontalCoordinate, edgeDistance);
			for (int i = 0; i <= edgeDistance; i++)
			{
				bitmap.SetPixel(horizontalCoordinate, i, pixel);
			}
			Color pixel2 = bitmap.GetPixel(horizontalCoordinate, bitmap.Height - edgeDistance - 2);
			for (int num = bitmap.Height - 1; num > bitmap.Height - edgeDistance - 2; num--)
			{
				bitmap.SetPixel(horizontalCoordinate, num, pixel2);
			}
		}

		private void InitializeSides()
		{
			if (HorizontalSide == null)
			{
				HorizontalSide = new RectangularFrameSide(Width, Margin, CornerRadius);
				VerticalSide = new RectangularFrameSide(Height, Margin, CornerRadius);
			}
		}
	}
}