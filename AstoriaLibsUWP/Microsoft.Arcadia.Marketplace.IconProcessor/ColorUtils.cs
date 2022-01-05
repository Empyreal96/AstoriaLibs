using System;
using System.Collections.Generic;
using Microsoft.Arcadia.Marketplace.IconProcessor.Imaging;

namespace Microsoft.Arcadia.Marketplace.IconProcessor
{
	public static class ColorUtils
	{
		private const int BucketsPerColor = 3;

		private const int BucketsTotal = 27;

		private const int GreyishThreshold = 48;

		private const int BucketRange = 86;

		private const int MaxHue = 240;

		private const int PercentThreshold = 7;

		private const int BareMinimumPixelCount = 5;

		private const float AcceptableHueLowerBound = 0.11f;

		private const float AcceptableHueUpperBound = 0.24f;

		private static Tuple<Color, int>[] colorizationPossibilities = new Tuple<Color, int>[9]
		{
		new Tuple<Color, int>(Color.FromArgb(byte.MaxValue, 123, 0, 0), 0),
		new Tuple<Color, int>(Color.FromArgb(byte.MaxValue, 210, 71, 38), 7),
		new Tuple<Color, int>(Color.FromArgb(byte.MaxValue, 0, 138, 0), 80),
		new Tuple<Color, int>(Color.FromArgb(byte.MaxValue, 18, 128, 35), 86),
		new Tuple<Color, int>(Color.FromArgb(byte.MaxValue, 0, 130, 153), 126),
		new Tuple<Color, int>(Color.FromArgb(byte.MaxValue, 9, 74, 178), 144),
		new Tuple<Color, int>(Color.FromArgb(byte.MaxValue, 81, 51, 171), 170),
		new Tuple<Color, int>(Color.FromArgb(byte.MaxValue, 140, 0, 149), 197),
		new Tuple<Color, int>(Color.FromArgb(byte.MaxValue, 172, 25, 61), 230)
		};

		public static Color GetDominantColorFromBitmap(Image bitmap)
		{
			if (bitmap == null)
			{
				throw new ArgumentNullException("bitmap");
			}
			Color result = Color.FromArgb(byte.MaxValue, 89, 89, 89);
			int num = 0;
			IList<Color>[] array = new List<Color>[27];
			for (int i = 0; i < 27; i++)
			{
				array[i] = new List<Color>();
			}
			for (int j = 0; j < bitmap.Height; j++)
			{
				for (int k = 0; k < bitmap.Width; k++)
				{
					Color pixel = bitmap.GetPixel(k, j);
					if (pixel.A > 0)
					{
						int r = pixel.R;
						int g = pixel.G;
						int b = pixel.B;
						if (Math.Max(r, Math.Max(g, b)) - Math.Min(r, Math.Min(g, b)) > 48)
						{
							int num2 = 9 * (r / 86) + 3 * (g / 86) + b / 86;
							array[num2].Add(pixel);
							num++;
						}
					}
				}
			}
			Array.Sort(array, delegate (IList<Color> x, IList<Color> y)
			{
				if (x.Count == y.Count)
				{
					return 0;
				}
				return (x.Count < y.Count) ? 1 : (-1);
			});
			if (num < 5)
			{
				return result;
			}
			for (int l = 0; l < array.Length; l++)
			{
				int count = array[l].Count;
				if (count * 100 / num < 7)
				{
					return result;
				}
				int num3 = 0;
				int num4 = 0;
				int num5 = 0;
				int num6 = 0;
				foreach (Color item in array[l])
				{
					num3 += item.R;
					num4 += item.G;
					num5 += item.B;
					num6 += item.A;
				}
				num3 /= count;
				num4 /= count;
				num5 /= count;
				num6 /= count;
				Color color = Color.FromArgb((byte)num6, (byte)num3, (byte)num4, (byte)num5);
				if (HueIsAcceptable(color))
				{
					return color;
				}
			}
			return result;
		}

		public static float GetHue(Color color)
		{
			if (color.R == color.G && color.G == color.B)
			{
				return 0f;
			}
			float num = (float)(int)color.R / 255f;
			float num2 = (float)(int)color.G / 255f;
			float num3 = (float)(int)color.B / 255f;
			float num4 = 0f;
			float num5 = num;
			float num6 = num;
			if (num2 > num5)
			{
				num5 = num2;
			}
			if (num3 > num5)
			{
				num5 = num3;
			}
			if (num2 < num6)
			{
				num6 = num2;
			}
			if (num3 < num6)
			{
				num6 = num3;
			}
			float num7 = num5 - num6;
			if (num7 == 0f)
			{
				return 0f;
			}
			if (num == num5)
			{
				num4 = (num2 - num3) / num7 + (float)((num2 < num3) ? 6 : 0);
			}
			else if (num2 == num5)
			{
				num4 = 2f + (num3 - num) / num7;
			}
			else if (num3 == num5)
			{
				num4 = 4f + (num - num2) / num7;
			}
			num4 = 256f * (num4 / 6f);
			if (num4 < 0f)
			{
				num4 += 360f;
			}
			return num4;
		}

		public static Color BlendColor(Color backgroundColor, Color foregroundColor)
		{
			if (backgroundColor.A == 0)
			{
				return foregroundColor;
			}
			byte b = (byte)(backgroundColor.R * backgroundColor.A / 255);
			byte b2 = (byte)(backgroundColor.G * backgroundColor.A / 255);
			byte b3 = (byte)(backgroundColor.B * backgroundColor.A / 255);
			double num = (double)(int)foregroundColor.A / 255.0;
			byte r = (byte)((double)(int)b * (1.0 - num) + (double)(int)foregroundColor.R * num);
			byte g = (byte)((double)(int)b2 * (1.0 - num) + (double)(int)foregroundColor.G * num);
			byte b4 = (byte)((double)(int)b3 * (1.0 - num) + (double)(int)foregroundColor.B * num);
			return Color.FromArgb(byte.MaxValue, r, g, b4);
		}

		internal static int ColorDistance(Color c1, Color c2)
		{
			return (int)Math.Sqrt((c1.A - c2.A) * (c1.A - c2.A) + (c1.R - c2.R) * (c1.R - c2.R) + (c1.G - c2.G) * (c1.G - c2.G) + (c1.B - c2.B) * (c1.B - c2.B));
		}

		internal static Color CalculateRecommendedBackgroundColor(Image bitmap)
		{
			Color dominantColorFromBitmap = GetDominantColorFromBitmap(bitmap);
			return GetClosestPossibleColor(dominantColorFromBitmap);
		}

		private static bool HueIsAcceptable(Color color)
		{
			float hue = GetHue(color);
			float num = hue / 240f;
			bool result = true;
			if (num >= 0.11f && num <= 0.24f)
			{
				result = false;
			}
			return result;
		}

		private static Color GetClosestPossibleColor(Color color)
		{
			int num = (int)GetHue(color);
			int num2 = 0;
			int num3 = 0;
			int num4 = num;
			bool flag = false;
			while (!flag)
			{
				for (int i = 0; i < colorizationPossibilities.Length; i++)
				{
					if (colorizationPossibilities[i].Item2 == num4)
					{
						num2 = i;
						flag = true;
						break;
					}
				}
				if (!flag)
				{
					num4 = ((num4 != 0) ? (num4 - 1) : 240);
					num3++;
					if (num3 >= 240)
					{
						break;
					}
				}
			}
			int num5 = 0;
			int num6 = 0;
			num4 = num;
			flag = false;
			while (!flag)
			{
				for (int j = 0; j < colorizationPossibilities.Length; j++)
				{
					if (colorizationPossibilities[j].Item2 == num4)
					{
						num5 = j;
						flag = true;
						break;
					}
				}
				if (!flag)
				{
					num4++;
					if (num4 > 240)
					{
						num4 = 0;
					}
					num6++;
					if (num3 >= 240)
					{
						break;
					}
				}
			}
			if (num3 <= num6)
			{
				return colorizationPossibilities[num2].Item1;
			}
			return colorizationPossibilities[num5].Item1;
		}
	}
}