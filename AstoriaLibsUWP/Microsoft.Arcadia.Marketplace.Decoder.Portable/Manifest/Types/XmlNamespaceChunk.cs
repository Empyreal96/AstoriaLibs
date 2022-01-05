using System;
using Microsoft.Arcadia.Marketplace.Decoder.Portable.Common;

namespace Microsoft.Arcadia.Marketplace.Decoder.Portable.Manifest.Types
{
	internal sealed class XmlNamespaceChunk : XmlItemChunk
	{
		public uint Prefix { get; set; }

		public uint Uri { get; set; }

		public XmlNamespaceChunk(ChunkType type)
		{
			if (type != ChunkType.ResXmlFirstChunkType && type != ChunkType.ResXmlEndNamespaceType)
			{
				throw new ApkDecoderManifestException("Invalid chunk type");
			}
			base.ChunkType = type;
			Prefix = 0u;
			Uri = 0u;
		}

		protected override void ParseBody(StreamDecoder streamDecoder)
		{
			if (streamDecoder == null)
			{
				throw new ArgumentNullException("streamDecoder");
			}
			streamDecoder.Offset += 8u;
			Prefix = streamDecoder.ReadUint32();
			Uri = streamDecoder.ReadUint32();
		}
	}
}