using System.Globalization;

namespace Microsoft.Arcadia.Marketplace.IconProcessor.Imaging
{
	public struct Color
	{
		public static Color Transparent => FromArgb(0, 0, 0, 0);

		public byte A { get; set; }

		public byte B { get; set; }

		public byte G { get; set; }

		public byte R { get; set; }

		public static bool operator ==(Color a, Color b)
		{
			if (a.A == b.A && a.B == b.B && a.G == b.G)
			{
				return a.R == b.R;
			}
			return false;
		}

		public static bool operator !=(Color a, Color b)
		{
			return !(a == b);
		}

		public static Color FromArgb(byte a, byte r, byte g, byte b)
		{
			Color result = default(Color);
			result.A = a;
			result.B = b;
			result.G = g;
			result.R = r;
			return result;
		}

		public override bool Equals(object obj)
		{
			if (obj is Color color)
			{
				return this == color;
			}
			return false;
		}

		public override int GetHashCode()
		{
			int num = 17;
			num = num * 23 + A.GetHashCode();
			num = num * 23 + B.GetHashCode();
			num = num * 23 + G.GetHashCode();
			return num * 23 + R.GetHashCode();
		}

		public override string ToString()
		{
			return string.Format(CultureInfo.InvariantCulture, "#{0}{1}{2}", new object[3]
			{
			R.ToString("X", CultureInfo.InvariantCulture).PadLeft(2, '0'),
			G.ToString("X", CultureInfo.InvariantCulture).PadLeft(2, '0'),
			B.ToString("X", CultureInfo.InvariantCulture).PadLeft(2, '0')
			});
		}
	}
}