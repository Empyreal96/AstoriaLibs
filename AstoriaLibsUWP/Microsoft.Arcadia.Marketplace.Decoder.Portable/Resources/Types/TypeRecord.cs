using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Microsoft.Arcadia.Marketplace.Decoder.Portable.Common;

namespace Microsoft.Arcadia.Marketplace.Decoder.Portable.Resources.Types
{
	internal sealed class TypeRecord
	{
		public TypeSpecChunk TypeSpecChunk { get; private set; }

		public List<TypeChunk> TypeChunks { get; private set; }

		public TypeRecord()
		{
			TypeSpecChunk = new TypeSpecChunk();
			TypeChunks = new List<TypeChunk>();
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder(string.Format(CultureInfo.InvariantCulture, "TypeRecord - TypeSpecChunk: {0}, TypeChunks Count: {1}", new object[2] { TypeSpecChunk, TypeChunks.Count }));
			stringBuilder.AppendLine("TypeChunks Items: ");
			foreach (TypeChunk typeChunk in TypeChunks)
			{
				stringBuilder.AppendFormat(CultureInfo.InvariantCulture, "{0}\n", new object[1] { typeChunk });
			}
			return stringBuilder.ToString();
		}

		public void Parse(StreamDecoder streamDecoder)
		{
			TypeSpecChunk.Parse(streamDecoder);
			while (streamDecoder.Offset < streamDecoder.Boundary && streamDecoder.PeakUint16() == 513)
			{
				TypeChunk typeChunk = new TypeChunk();
				typeChunk.Parse(streamDecoder);
				TypeChunks.Add(typeChunk);
			}
		}
	}
}