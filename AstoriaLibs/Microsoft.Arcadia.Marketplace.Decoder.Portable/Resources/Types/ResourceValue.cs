using System;
using System.Globalization;
using Microsoft.Arcadia.Marketplace.Decoder.Portable.Common;

namespace Microsoft.Arcadia.Marketplace.Decoder.Portable.Resources.Types
{
	internal sealed class ResourceValue
	{
		public ushort Size { get; private set; }

		public ResourceValueTypes Type { get; private set; }

		public uint Data { get; private set; }

		public ResourceValue()
		{
			Size = 0;
			Type = ResourceValueTypes.None;
			Data = 0; // originally 0u
		}

		public override string ToString()
		{
			return string.Format(CultureInfo.InvariantCulture, "ResourceValue - Size: {0}, Type: {1}, Data: {2}", new object[3] { Size, Type, Data });
		}

		public void Parse(StreamDecoder streamDecoder)
		{
			if (streamDecoder == null)
			{
				throw new ArgumentNullException("streamDecoder");
			}
			Size = streamDecoder.ReadUint16();
			streamDecoder.Offset++;
			Type = (ResourceValueTypes)streamDecoder.ReadByte();
			Data = streamDecoder.ReadUint32();
		}
	}
}