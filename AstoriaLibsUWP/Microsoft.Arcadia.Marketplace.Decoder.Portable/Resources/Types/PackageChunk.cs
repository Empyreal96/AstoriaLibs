using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Microsoft.Arcadia.Marketplace.Decoder.Portable.Common;
using Microsoft.Arcadia.Marketplace.Utils.Log;

namespace Microsoft.Arcadia.Marketplace.Decoder.Portable.Resources.Types
{
	internal sealed class PackageChunk : Chunk
	{
		private const ushort MaxPackageNameInChars = 128;

		private uint typeStrings;

		private uint lastPublicType;

		private uint keyStrings;

		private uint lastPublicKey;

		public uint PackageId { get; private set; }

		public string PackageName { get; private set; }

		public StringPoolChunk TypeNameStringsChunk { get; private set; }

		public StringPoolChunk TypeKeyStringsChunk { get; private set; }

		public List<TypeRecord> TypeRecords { get; private set; }

		public PackageChunk()
		{
			base.ChunkType = ChunkType.ResTablePackageType;
			PackageId = 0u;
			TypeNameStringsChunk = new StringPoolChunk();
			TypeKeyStringsChunk = new StringPoolChunk();
			TypeRecords = new List<TypeRecord>();
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder("PackageChunk Info: \n");
			stringBuilder.AppendFormat(CultureInfo.InvariantCulture, "\t Offset to string pool header: {0}, Last index into type strings: {1}, Offset to the resource key symbol table: {2}, Last index into key strings: {3}", typeStrings, lastPublicType, keyStrings, lastPublicKey);
			stringBuilder.AppendFormat(CultureInfo.InvariantCulture, "\t Name: {0}, Id: {1}, TypeNameStringsChunk Count: {2}, TypeKeyStringsChunk Count: {3}, TypeRecords Count: {4}\n", PackageName, PackageId, TypeNameStringsChunk.Strings.Count, TypeKeyStringsChunk.Strings.Count, TypeRecords.Count);
			stringBuilder.AppendFormat(CultureInfo.InvariantCulture, "TypeKeyStringsChunk: {0}", new object[1] { TypeKeyStringsChunk });
			stringBuilder.AppendFormat(CultureInfo.InvariantCulture, "TypeNameStringsChunk: {0}", new object[1] { TypeNameStringsChunk });
			stringBuilder.AppendFormat(CultureInfo.InvariantCulture, "TypeRecords: \n");
			foreach (TypeRecord typeRecord in TypeRecords)
			{
				stringBuilder.AppendFormat(CultureInfo.InvariantCulture, "\t, {0} \n", new object[1] { typeRecord });
			}
			return stringBuilder.ToString();
		}

		protected override void ParseBody(StreamDecoder streamDecoder)
		{
			PackageId = streamDecoder.ReadUint32();
			PackageName = ReadPackageName(streamDecoder);
			typeStrings = streamDecoder.ReadUint32();
			lastPublicType = streamDecoder.ReadUint32();
			keyStrings = streamDecoder.ReadUint32();
			lastPublicKey = streamDecoder.ReadUint32();
			streamDecoder.Offset = base.BaseOffset + typeStrings;
			TypeNameStringsChunk.Parse(streamDecoder);
			streamDecoder.Offset = base.BaseOffset + keyStrings;
			TypeKeyStringsChunk.Parse(streamDecoder);
			while (streamDecoder.Offset < base.BaseOffset + base.ChunkSize)
			{
				TypeRecord typeRecord = new TypeRecord();
				typeRecord.Parse(streamDecoder);
				TypeRecords.Add(typeRecord);
			}
			LoggerCore.Log("Name: {0}, Id: {1}, TypeNameStringsChunk Count: {2}, TypeKeyStringsChunk Count: {3}, TypeRecords Count: {4}", PackageName, PackageId, TypeNameStringsChunk.Strings.Count, TypeKeyStringsChunk.Strings.Count, TypeRecords.Count);
		}

		private static string ReadPackageName(StreamDecoder streamDecoder)
		{
			uint offset = streamDecoder.Offset;
			char[] array = new char[128];
			int i;
			for (i = 0; i < 128; i++)
			{
				array[i] = (char)streamDecoder.ReadUint16();
				if (array[i] == '\0')
				{
					break;
				}
			}
			if (array[i] != 0)
			{
				throw new ApkDecoderResourcesException("Package name isn't null terminated");
			}
			string result = new string(array, 0, i);
			streamDecoder.Offset = offset + 256;
			return result;
		}
	}
}