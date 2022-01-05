using System;
using System.Collections.Generic;
using Microsoft.Arcadia.Marketplace.IconProcessor.Imaging;

namespace Microsoft.Arcadia.Marketplace.IconProcessor
{
	internal class IconSpectrum
	{
		private List<IconSpectrumPoint> points;

		internal bool HasRoundedEdge { get; private set; }

		internal bool HasSquareEdge { get; private set; }

		internal bool HasUniformColor { get; private set; }

		internal Color UniformColor { get; private set; }

		internal int Margin { get; private set; }

		internal Point MainTraversalVector { get; private set; }

		internal Point ScanVector { get; private set; }

		internal Point InitVector { get; private set; }

		internal IconSpectrum(Point initVector, Point mainTraversalVector, Point scanVector)
		{
			points = new List<IconSpectrumPoint>();
			InitVector = initVector;
			MainTraversalVector = mainTraversalVector;
			ScanVector = scanVector;
		}

		internal void Sample(Image image, RectangularFrameSide frame, Color backgroundColor)
		{
			Point point = new Point(InitVector.X, InitVector.Y);
			int num = 0;
			int num2 = 0;
			while (point.X < image.Width && point.Y < image.Height && point.X >= 0 && point.Y >= 0)
			{
				int num3 = 0;
				bool flag = false;
				int num4 = frame?.GetDistanceFromTheEdge(num) ?? 0;
				while (point.X < image.Width && point.Y < image.Height && point.X >= 0 && point.Y >= 0)
				{
					Color pixel = image.GetPixel(point.X, point.Y);
					if (pixel.A >= 224)
					{
						if (!flag)
						{
							points.Add(new IconSpectrumPoint(num3, pixel));
							flag = true;
						}
						if (num3 < num4)
						{
							Color c = ColorUtils.BlendColor(backgroundColor, pixel);
							int num5 = ColorUtils.ColorDistance(backgroundColor, c);
							if (num5 > 20)
							{
								num2++;
							}
						}
					}
					if (flag && num3 >= num4)
					{
						break;
					}
					point.X += ScanVector.X;
					point.Y += ScanVector.Y;
					num3++;
				}
				if (ScanVector.X != 0)
				{
					point.X = InitVector.X;
				}
				if (ScanVector.Y != 0)
				{
					point.Y = InitVector.Y;
				}
				point.X += MainTraversalVector.X;
				point.Y += MainTraversalVector.Y;
				num++;
			}
			if (num2 > num * 100 / 5)
			{
				HasUniformColor = false;
			}
			else
			{
				HasUniformColor = true;
			}
		}

		internal void AnalyzeEdge(int startLeft, int stopRight)
		{
			if (points.Count == 0)
			{
				return;
			}
			bool hasSquareEdge = true;
			bool hasRoundedEdge = true;
			int num = points.Count * 25 / 100;
			IconSpectrumPoint iconSpectrumPoint = points[points.Count / 2];
			for (int i = startLeft; i < points.Count - stopRight; i++)
			{
				if (points[i].Margin == iconSpectrumPoint.Margin)
				{
					continue;
				}
				if (i > num && i < points.Count - num)
				{
					hasSquareEdge = false;
					hasRoundedEdge = false;
					continue;
				}
				if (Math.Abs(points[i].Margin - iconSpectrumPoint.Margin) > num)
				{
					hasRoundedEdge = false;
				}
				hasSquareEdge = false;
			}
			HasSquareEdge = hasSquareEdge;
			HasRoundedEdge = hasRoundedEdge;
		}

		internal void AnalyzeMargin()
		{
			int num = int.MaxValue;
			for (int i = 0; i < points.Count; i++)
			{
				if (points[i].Margin < num)
				{
					num = points[i].Margin;
				}
			}
			if (num == int.MaxValue)
			{
				num = 0;
			}
			Margin = num;
		}
	}
}