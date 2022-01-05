using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Microsoft.Arcadia.Marketplace.Decoder.Portable.Common
{
	internal sealed class StringPoolChunk : Chunk
	{
		private readonly List<string> strings;

		private uint flags;

		public IReadOnlyList<string> Strings => strings;

		public StringPoolChunk()
		{
			base.ChunkType = ChunkType.ResStringPoolType;
			strings = new List<string>();
			flags = 0u;
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder(string.Format(CultureInfo.InvariantCulture, "StringPoolChunk - Count: {0}, Flags: {1}, Strings: \n", new object[2] { strings.Count, flags }));
			foreach (string @string in strings)
			{
				_ = @string;
				stringBuilder.AppendFormat(CultureInfo.InvariantCulture, "\t {0} \n");
			}
			return stringBuilder.ToString();
		}

		protected override void ParseBody(StreamDecoder streamDecoder)
		{
			if (streamDecoder == null)
			{
				throw new ArgumentNullException("streamDecoder");
			}
			uint num = streamDecoder.ReadUint32();
			streamDecoder.Offset += 4u;
			flags = streamDecoder.ReadUint32();
			uint num2 = streamDecoder.ReadUint32();
			streamDecoder.ReadUint32();
			uint offset = streamDecoder.Offset;
			bool isUtf = (flags & 0x100) != 0;
			checked
			{
				for (uint num3 = 0u; num3 < num; num3++)
				{
					streamDecoder.Offset = offset + num3 * 4u;
					uint num4 = streamDecoder.ReadUint32();
					uint num6 = (streamDecoder.Offset = base.BaseOffset + num2 + num4);
					string item = streamDecoder.ReadString(isUtf);
					strings.Add(item);
				}
			}
		}
	}
}