using System;
using System.Collections.Generic;
using Microsoft.Arcadia.Marketplace.Decoder.Portable.Common;

namespace Microsoft.Arcadia.Marketplace.Decoder.Portable.Manifest.Types
{
	internal sealed class XmlStartElementChunk : XmlElementChunk
	{
		private const ushort XmlTreeAttributeSizeInBytes = 20;

		private readonly List<XmlAttribute> attributes;

		public ushort AttributeStart { get; set; }

		public ushort AttributeSize { get; set; }

		public ushort AttributeCount { get; set; }

		public ushort IdIndex { get; set; }

		public ushort ClassIndex { get; set; }

		public ushort StyleIndex { get; set; }

		public IReadOnlyCollection<XmlAttribute> Attributes => attributes;

		public XmlStartElementChunk()
		{
			base.ChunkType = ChunkType.ResXmlStartElementType;
			AttributeStart = 0;
			AttributeSize = 0;
			AttributeCount = 0;
			IdIndex = 0;
			ClassIndex = 0;
			StyleIndex = 0;
			attributes = new List<XmlAttribute>();
		}

		protected override void ParseBody(StreamDecoder streamDecoder)
		{
			if (streamDecoder == null)
			{
				throw new ArgumentNullException("streamDecoder");
			}
			streamDecoder.Offset += 8u;
			base.Namespace = streamDecoder.ReadUint32();
			base.Name = streamDecoder.ReadUint32();
			AttributeStart = streamDecoder.ReadUint16();
			AttributeSize = streamDecoder.ReadUint16();
			AttributeCount = streamDecoder.ReadUint16();
			IdIndex = streamDecoder.ReadUint16();
			ClassIndex = streamDecoder.ReadUint16();
			StyleIndex = streamDecoder.ReadUint16();
			if (AttributeSize != 20)
			{
				throw new ApkDecoderManifestException("Attribute Size has unexpected value: " + AttributeSize);
			}
			for (uint num = 0u; num < AttributeCount; num++)
			{
				XmlAttribute xmlAttribute = new XmlAttribute();
				xmlAttribute.Parse(streamDecoder);
				attributes.Add(xmlAttribute);
			}
		}
	}
}