using System;

namespace Microsoft.Arcadia.Marketplace.Decoder.Portable.Common
{
	internal abstract class Chunk
	{
		private const uint ChunkHeaderSizeInBytes = 8u;

		public ChunkType ChunkType { get; set; }

		protected uint HeaderSize { get; set; }

		protected uint ChunkSize { get; set; }

		protected uint BaseOffset { get; set; }

		protected Chunk()
		{
			ChunkType = ChunkType.None;
			HeaderSize = 0;// 0u
			ChunkSize = 0; //0u
			BaseOffset = 0; //0u
		}

		public void Parse(StreamDecoder streamDecoder)
		{
			if (streamDecoder == null)
			{
				throw new ArgumentNullException("streamDecoder");
			}
			if (ChunkType == ChunkType.None)
			{
				throw new ApkDecoderCommonException("Chunk Type must be set to a non-None value.");
			}
			BaseOffset = streamDecoder.Offset;
			ParseHeader(streamDecoder);
			checked
			{
				streamDecoder.PushReadBoundary(BaseOffset + ChunkSize);
				ParseBody(streamDecoder);
				streamDecoder.Offset = BaseOffset + ChunkSize;
				streamDecoder.PopReadBoundary();
			}
		}

		protected abstract void ParseBody(StreamDecoder streamDecoder);

		private void ParseHeader(StreamDecoder streamDecoder)
		{
			ChunkType chunkType = (ChunkType)streamDecoder.ReadUint16();
			if (chunkType != ChunkType)
			{
				throw new ApkDecoderCommonException(string.Concat("Unexpected chunk type, expected: ", ChunkType, ", actual: ", chunkType));
			}
			HeaderSize = streamDecoder.ReadUint16();
			if (HeaderSize < 8)
			{
				throw new ApkDecoderCommonException("Invalid header size");
			}
			ChunkSize = streamDecoder.ReadUint32();
			if (ChunkSize < HeaderSize)
			{
				throw new ApkDecoderCommonException("Total size should not be less than header size");
			}
		}
	}
}