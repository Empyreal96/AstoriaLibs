using System;
using System.Globalization;
using System.Text;

namespace Microsoft.Arcadia.Debugging.SharedUtils.Portable
{
	public static class IntegerHelper
	{
		public static uint Read32BitValueFromLittleEndianBytes(byte[] buffer, int offset)
		{
			if (buffer == null)
			{
				throw new ArgumentNullException("buffer");
			}
			if (offset < 0)
			{
				throw new ArgumentException("offset must be no less than 0", "offset");
			}
			if (offset + 4 > buffer.Length)
			{
				throw new ArgumentException("offset out of the boundary", "offset");
			}
			return (uint)((buffer[offset + 3] << 24) | (buffer[offset + 2] << 16) | (buffer[offset + 1] << 8) | buffer[offset]);
		}

		public static void WriteUintToLittleEndianBytes(uint value, byte[] buffer, int offset)
		{
			if (buffer == null)
			{
				throw new ArgumentNullException("buffer");
			}
			if (offset < 0)
			{
				throw new ArgumentException("offset must be no less than 0", "offset");
			}
			if (offset + 4 > buffer.Length)
			{
				throw new ArgumentOutOfRangeException("offset");
			}
			buffer[offset] = (byte)(value & 0xFFu);
			buffer[offset + 1] = (byte)((value >> 8) & 0xFFu);
			buffer[offset + 2] = (byte)((value >> 16) & 0xFFu);
			buffer[offset + 3] = (byte)((value >> 24) & 0xFFu);
		}

		public static string GetAsciiStringFromInteger(uint input)
		{
			StringBuilder stringBuilder = new StringBuilder();
			for (int i = 0; i < 4; i++)
			{
				char c = (char)((input >> i * 8) & 0xFFu);
				if (IsPrintableAsciiChar(c))
				{
					stringBuilder.Append(c);
					continue;
				}
				string value = string.Format(CultureInfo.InvariantCulture, "[unprintable: {0}]", new object[1] { (int)c });
				stringBuilder.Append(value);
			}
			return stringBuilder.ToString();
		}

		private static bool IsPrintableAsciiChar(char candidate)
		{
			if (candidate >= ' ')
			{
				return candidate < '\u007f';
			}
			return false;
		}
	}
}
