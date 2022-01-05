using System.Globalization;
using Microsoft.Arcadia.Marketplace.Decoder.Portable.Common;

namespace Microsoft.Arcadia.Marketplace.Decoder.Portable.Resources.Types
{
	internal sealed class ResourceKey
	{
		private const ushort ResTableEntryInBytes = 8;

		private const ushort ResTableMapEntryInBytes = 16;

		public uint Size { get; private set; }

		public uint Flag { get; private set; }

		public uint Key { get; private set; }

		public uint Parent { get; private set; }

		public uint Count { get; private set; }

		public ResourceKey()
		{
			Size = 0; //originally 0u
			Flag = 0; //originally 0u
			Key = 0; //originally 0u
			Parent = 0; //originally 0u
			Count = 0; //originally 0u
		}

		public override string ToString()
		{
			return string.Format(CultureInfo.InvariantCulture, "ResourceKey - Size: {0}, Flag: {1}, Key: {2}, Parent: {3}, Count: {4}", Size, Flag, Key, Parent, Count);
		}

		public void Parse(StreamDecoder streamDecoder)
		{
			ushort num = streamDecoder.ReadUint16();
			if (num != 8 && num != 16)
			{
				throw new ApkDecoderResourcesException("Resource Table size in bytes has unexpected value: " + num);
			}
			Size = num;
			Flag = streamDecoder.ReadUint16();
			Key = streamDecoder.ReadUint32();
			if (Size == 16)
			{
				Parent = streamDecoder.ReadUint32();
				Count = streamDecoder.ReadUint32();
			}
		}

		public bool IsComplexValue()
		{
			return (Flag & 1) != 0;
		}
	}
}