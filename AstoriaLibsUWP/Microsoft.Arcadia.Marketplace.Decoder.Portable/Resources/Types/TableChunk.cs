using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Microsoft.Arcadia.Marketplace.Decoder.Portable.Common;

namespace Microsoft.Arcadia.Marketplace.Decoder.Portable.Resources.Types
{
	internal sealed class TableChunk : Chunk
	{
		public StringPoolChunk StringPoolChunk { get; private set; }

		public List<PackageChunk> PackageChunkList { get; private set; }

		public TableChunk()
		{
			base.ChunkType = ChunkType.ResTableType;
			StringPoolChunk = new StringPoolChunk();
			PackageChunkList = new List<PackageChunk>();
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendFormat(CultureInfo.InvariantCulture, "TableChunk - Contains: StringPoolChunk Count: {0}, PackageChunks Count: {1}", new object[2]
			{
			StringPoolChunk.Strings.Count,
			PackageChunkList.Count
			});
			stringBuilder.Append(StringPoolChunk);
			stringBuilder.AppendLine();
			foreach (PackageChunk packageChunk in PackageChunkList)
			{
				stringBuilder.AppendFormat(CultureInfo.InvariantCulture, "{0}\n", new object[1] { packageChunk });
			}
			return stringBuilder.ToString();
		}

		protected override void ParseBody(StreamDecoder streamDecoder)
		{
			uint num = streamDecoder.ReadUint32();
			StringPoolChunk.Parse(streamDecoder);
			for (uint num2 = 0u; num2 < num; num2++)
			{
				PackageChunk packageChunk = new PackageChunk();
				packageChunk.Parse(streamDecoder);
				PackageChunkList.Add(packageChunk);
			}
		}
	}
}