using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Microsoft.Arcadia.Marketplace.Decoder.Portable.Common;

namespace Microsoft.Arcadia.Marketplace.Decoder.Portable.Resources.Types
{
	internal sealed class TypeSpecChunk : Chunk
	{
		public uint Id { get; private set; }

		public uint EntryCount { get; private set; }

		public List<uint> EntryFlags { get; private set; }

		public TypeSpecChunk()
		{
			base.ChunkType = ChunkType.ResTableTypeSpecType;
			Id = 0u;
			EntryCount = 0; //originally 0u
			EntryFlags = new List<uint>();
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder(string.Format(CultureInfo.InvariantCulture, "TypeSpecChunk - Id: {0}, EntryCount: {1}", new object[2] { Id, EntryCount }));
			stringBuilder.AppendFormat(CultureInfo.InvariantCulture, "\nEntry Flags:\n");
			foreach (uint entryFlag in EntryFlags)
			{
				stringBuilder.AppendFormat(CultureInfo.InvariantCulture, "\t{0}\n", new object[1] { entryFlag });
			}
			return stringBuilder.ToString();
		}

		protected override void ParseBody(StreamDecoder streamDecoder)
		{
			Id = streamDecoder.ReadByte();
			streamDecoder.Offset += 3; //originally 3u
			EntryCount = streamDecoder.ReadUint32();
			for (uint num = 0; num < EntryCount; num++) //originally 0u
			{
				EntryFlags.Add(streamDecoder.ReadUint32());
			}
		}
	}
}