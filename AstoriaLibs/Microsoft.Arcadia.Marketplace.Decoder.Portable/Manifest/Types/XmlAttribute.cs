using Microsoft.Arcadia.Marketplace.Decoder.Portable.Common;
using Microsoft.Arcadia.Marketplace.Decoder.Portable.Resources.Types;

namespace Microsoft.Arcadia.Marketplace.Decoder.Portable.Manifest.Types
{
	internal class XmlAttribute
	{
		public uint Namespace { get; protected set; }

		public uint Name { get; protected set; }

		public ResourceValue Data { get; protected set; }

		protected uint RawValue { get; set; }

		public XmlAttribute()
		{
			Namespace = uint.MaxValue;
			Name = 0u;
			RawValue = 0u;
			Data = new ResourceValue();
		}

		public void Parse(StreamDecoder streamDecoder)
		{
			Namespace = streamDecoder.ReadUint32();
			Name = streamDecoder.ReadUint32();
			RawValue = streamDecoder.ReadUint32();
			Data.Parse(streamDecoder);
		}
	}
}