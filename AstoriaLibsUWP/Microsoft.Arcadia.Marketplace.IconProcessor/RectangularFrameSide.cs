namespace Microsoft.Arcadia.Marketplace.IconProcessor
{
	internal class RectangularFrameSide
	{
		internal int CornerRadius { get; private set; }

		internal int Margin { get; set; }

		internal int TotalEdgeLength { get; set; }

		internal int EdgeTestStart => Margin + CornerRadius / 2;

		internal int EdgeTestEnd => TotalEdgeLength - Margin - CornerRadius / 2;

		internal RectangularFrameSide(int totalLength, int margin, int radius)
		{
			TotalEdgeLength = totalLength;
			Margin = margin;
			CornerRadius = radius;
		}

		internal int GetDistanceFromTheEdge(int y)
		{
			int result = Margin;
			if (y < Margin + CornerRadius)
			{
				result = Margin * 2 + CornerRadius - y;
			}
			else if (y > TotalEdgeLength - CornerRadius - Margin)
			{
				result = Margin + y - (TotalEdgeLength - CornerRadius - Margin);
			}
			return result;
		}
	}
}