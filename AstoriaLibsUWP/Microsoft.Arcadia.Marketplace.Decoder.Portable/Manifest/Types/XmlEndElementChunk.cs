using System;
using Microsoft.Arcadia.Marketplace.Decoder.Portable.Common;

namespace Microsoft.Arcadia.Marketplace.Decoder.Portable.Manifest.Types
{
	internal sealed class XmlEndElementChunk : XmlElementChunk
	{
		public XmlEndElementChunk()
		{
			base.ChunkType = ChunkType.ResXmlEndElementType;
		}

		protected override void ParseBody(StreamDecoder streamDecoder)
		{
			if (streamDecoder == null)
			{
				throw new ArgumentNullException("streamDecoder");
			}
			streamDecoder.Offset += 8; //originally 8u
			base.Namespace = streamDecoder.ReadUint32();
			base.Name = streamDecoder.ReadUint32();
		}
	}
}