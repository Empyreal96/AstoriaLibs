using System;
using System.Globalization;
using Microsoft.Arcadia.Marketplace.Decoder.Portable.Common;
using Microsoft.Arcadia.Marketplace.Utils.Log;

namespace Microsoft.Arcadia.Marketplace.Decoder.Portable.Resources.Types
{
	internal sealed class ResourceConfig
	{
		private const ushort MaxResTableConfigInBytes = 36;

		public uint Size { get; private set; }

		public uint Imsi { get; private set; }

		public string Locale { get; private set; }

		public uint ScreenType { get; private set; }

		public uint Input { get; private set; }

		public uint ScreenSize { get; private set; }

		public uint Version { get; private set; }

		public uint ScreenConfig { get; private set; }

		public uint ScreenSizeDp { get; private set; }

		public override string ToString()
		{
			return string.Format(CultureInfo.InvariantCulture, "ResourceConfig - Size: {0}, Imsi: {1}, locale: {2}, ScreenType: {3}, Input: {4}, ScreenSize: {5}, Version: {6}, ScreenConfig: {7}, ScreenSizeDp: {8}", Size, Imsi, Locale, ScreenType, Input, ScreenSize, Version, ScreenConfig, ScreenSizeDp);
		}

		public void Parse(StreamDecoder streamDecoder)
		{
			uint offset = streamDecoder.Offset;
			Size = streamDecoder.ReadUint32();
			offset += Size;
			if (Size > 36)
			{
				LoggerCore.Log("Resource config size has unexpected value: {0}, (Expected: {1}", Size, (ushort)36);
			}
			uint num = Size;
			if (num != 0)
			{
				Imsi = streamDecoder.ReadUint32();
				num -= 4;
			}
			if (num != 0)
			{
				uint localeValue = streamDecoder.ReadUint32();
				Locale = GetLocaleAsString(localeValue);
				num -= 4;
			}
			if (num != 0)
			{
				ScreenType = streamDecoder.ReadUint32();
				num -= 4;
			}
			if (num != 0)
			{
				Input = streamDecoder.ReadUint32();
				num -= 4;
			}
			if (num != 0)
			{
				ScreenSize = streamDecoder.ReadUint32();
				num -= 4;
			}
			if (num != 0)
			{
				Version = streamDecoder.ReadUint32();
				num -= 4;
			}
			if (num != 0)
			{
				ScreenConfig = streamDecoder.ReadUint32();
				num -= 4;
			}
			if (num != 0)
			{
				ScreenSizeDp = streamDecoder.ReadUint32();
			}
			streamDecoder.Offset = offset;
		}

		private static string GetLocaleAsString(uint localeValue)
		{
			char[] array = new char[6];
			int num = 0;
			array[0] = Convert.ToChar(localeValue & 0x7Fu);
			if (array[0] != 0)
			{
				num++;
				array[1] = Convert.ToChar((localeValue >> 8) & 0x7Fu);
				if (array[1] != 0)
				{
					num++;
					array[3] = Convert.ToChar((localeValue >> 16) & 0x7Fu);
					if (array[3] != 0)
					{
						num++;
						array[2] = '-';
						num++;
						array[4] = Convert.ToChar((localeValue >> 24) & 0x7Fu);
						if (array[4] != 0)
						{
							num++;
							array[5] = '\0';
						}
					}
				}
			}
			if (array[0] != 0)
			{
				return new string(array, 0, num);
			}
			return string.Empty;
		}
	}
}