using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Microsoft.Arcadia.Marketplace.Decoder.Portable.Common;

namespace Microsoft.Arcadia.Marketplace.Decoder.Portable.Resources.Types
{
	internal sealed class TypeChunk : Chunk
	{
		private const uint ResTableTypeNoEntry = uint.MaxValue;

		public uint Id { get; private set; }

		public uint EntryCount { get; private set; }

		public uint EntriesStart { get; private set; }

		public ResourceConfig Config { get; private set; }

		public Dictionary<uint, ResourceItem> ResourceItems { get; private set; }

		public TypeChunk()
		{
			base.ChunkType = ChunkType.ResTableTypeType;
			Id = 0u;
			EntryCount = 0; //originally 0u
			EntriesStart = 0; //originally 0u
			Config = new ResourceConfig();
			ResourceItems = new Dictionary<uint, ResourceItem>();
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder(string.Format(CultureInfo.InvariantCulture, "TypeChunk - Id: {0}, EntryCount: {1}, EntriesStart: {2}, Config: {3}, Resource Items Count: {4}\n", Id, EntryCount, EntriesStart, Config, ResourceItems.Count));
			stringBuilder.AppendLine("Resource Items:");
			foreach (KeyValuePair<uint, ResourceItem> resourceItem in ResourceItems)
			{
				stringBuilder.AppendFormat(CultureInfo.InvariantCulture, "{0}:{1}", new object[2] { resourceItem.Key, resourceItem.Value });
			}
			return stringBuilder.ToString();
		}

		protected override void ParseBody(StreamDecoder streamDecoder)
		{
			Id = streamDecoder.ReadByte();
			streamDecoder.Offset += 3u;
			EntryCount = streamDecoder.ReadUint32();
			EntriesStart = streamDecoder.ReadUint32();
			Config.Parse(streamDecoder);
			for (uint num = 0; num < EntryCount; num++) //originally 0u
			{
				uint num2 = streamDecoder.ReadUint32();
				if (num2 != uint.MaxValue)
				{
					uint offset = streamDecoder.Offset;
					streamDecoder.Offset = base.BaseOffset + EntriesStart + num2;
					ResourceItem resourceItem = new ResourceItem();
					resourceItem.Parse(streamDecoder);
					ResourceItems.Add(num, resourceItem);
					streamDecoder.Offset = offset;
				}
			}
		}
	}
}