using System;
using Microsoft.Arcadia.Marketplace.Decoder.Portable.Manifest.Types;
using Microsoft.Arcadia.Marketplace.Decoder.Portable.Resources.Types;
using Microsoft.Arcadia.Marketplace.Utils.Log;

namespace Microsoft.Arcadia.Marketplace.Decoder.Portable.Common
{
	internal sealed class ChunkDecoder
	{

		public static Chunk Decode(StreamDecoder streamDecoder)
		{
			if (streamDecoder == null)
			{
				throw new ArgumentNullException("streamDecoder");
			}
			ushort num = streamDecoder.PeakUint16();
			LoggerCore.Log("Chunk Type: {0} ({1})", (ChunkType)num, num);
			
			Chunk chunk;
			switch(num)
				{
				case 1:
					chunk = new StringPoolChunk();
					break;
				case 2:
					chunk = new XmlChunk();
					break;
				case 3:
					chunk = new TableChunk();
					break;
				default:
					throw new ApkDecoderCommonException("Unrecognized chunk type" + num);

			};
			chunk.Parse(streamDecoder);
			return chunk;
		}

	}
}