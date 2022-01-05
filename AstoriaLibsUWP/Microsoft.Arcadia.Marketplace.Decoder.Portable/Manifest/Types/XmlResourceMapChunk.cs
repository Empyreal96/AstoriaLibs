using System;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.Arcadia.Marketplace.Decoder.Portable.Common;
using Microsoft.Arcadia.Marketplace.Utils.Log;

namespace Microsoft.Arcadia.Marketplace.Decoder.Portable.Manifest.Types
{
	internal sealed class XmlResourceMapChunk : Chunk
	{
		public readonly List<uint> ResourceIds;

		public XmlResourceMapChunk()
		{
			base.ChunkType = ChunkType.ResXmlResourceMapType;
			ResourceIds = new List<uint>();
		}

		protected override void ParseBody(StreamDecoder streamDecoder)
		{
			if (streamDecoder == null)
			{
				throw new ArgumentNullException("streamDecoder");
			}
			LoggerCore.Log(string.Format(CultureInfo.InvariantCulture, "Chunk size: {0} Header size: {1}", new object[2] { base.ChunkSize, base.ChunkSize }));
			if ((base.ChunkSize - base.HeaderSize) % 4u != 0)
			{
				throw new ApkDecoderManifestException("The size of XML Resource Map Chunk Body is expected to be the multiple of 4");
			}
			for (uint num = base.HeaderSize; num < base.ChunkSize; num += 4)
			{
				ResourceIds.Add(streamDecoder.ReadUint32());
			}
		}
	}
}