using System;
using Microsoft.Arcadia.Marketplace.Decoder.Portable.Common;
using Microsoft.Arcadia.Marketplace.Decoder.Portable.Resources.Types;

namespace Microsoft.Arcadia.Marketplace.Decoder.Portable.Manifest.Types
{
	internal sealed class XmlCDataChunk : XmlItemChunk
	{
		private readonly ResourceValue typedData;

		public uint Data { get; private set; }

		public XmlCDataChunk()
		{
			typedData = new ResourceValue();
			base.ChunkType = ChunkType.ResXmlCDataType;
		}

		protected override void ParseBody(StreamDecoder streamDecoder)
		{
			if (streamDecoder == null)
			{
				throw new ArgumentNullException("streamDecoder");
			}
			streamDecoder.Offset += 8; //originally 8u
			Data = streamDecoder.ReadUint32();
			typedData.Parse(streamDecoder);
		}
	}
}