using System.Collections.Generic;
using Microsoft.Arcadia.Marketplace.Decoder.Portable.Common;
using Microsoft.Arcadia.Marketplace.Utils.Log;

namespace Microsoft.Arcadia.Marketplace.Decoder.Portable.Manifest.Types
{
	internal sealed class XmlChunk : Chunk
	{
		private readonly List<XmlItemChunk> xmlItemChunkList;

		public StringPoolChunk ChunkStringPool { get; private set; }

		public XmlResourceMapChunk XmlResourceMapChunk { get; private set; }

		public IReadOnlyCollection<XmlItemChunk> XmlItemChunkList => xmlItemChunkList;

		public XmlChunk()
		{
			base.ChunkType = ChunkType.ResXmlType;
			ChunkStringPool = new StringPoolChunk();
			XmlResourceMapChunk = new XmlResourceMapChunk();
			xmlItemChunkList = new List<XmlItemChunk>();
		}

		protected override void ParseBody(StreamDecoder streamDecoder)
		{
			ChunkStringPool.Parse(streamDecoder);
			ChunkType chunkType = (ChunkType)streamDecoder.PeakUint16();
			LoggerCore.Log("Chunk type: " + chunkType);
			if (chunkType == ChunkType.ResXmlResourceMapType)
			{
				XmlResourceMapChunk.Parse(streamDecoder);
			}
			uint num = 0; //originally 0u
			uint num2 = 0; //originally 0u
			while (streamDecoder.Offset < base.BaseOffset + base.ChunkSize)
			{
				XmlItemChunk xmlItemChunk;
				switch (streamDecoder.PeakUint16())
				{
					case 256:
						xmlItemChunk = new XmlNamespaceChunk(ChunkType.ResXmlFirstChunkType);
						num2++;
						break;
					case 257:
						xmlItemChunk = new XmlNamespaceChunk(ChunkType.ResXmlEndNamespaceType);
						num2--;
						break;
					case 258:
						xmlItemChunk = new XmlStartElementChunk();
						num++;
						break;
					case 259:
						xmlItemChunk = new XmlEndElementChunk();
						num--;
						break;
					case 260:
						xmlItemChunk = new XmlCDataChunk();
						break;
					default:
						xmlItemChunk = null;
						break;
				}
				if (xmlItemChunk == null)
				{
					break;
				}
				xmlItemChunk.Parse(streamDecoder);
				xmlItemChunkList.Add(xmlItemChunk);
			}
			if (num != 0)
			{
				throw new ApkDecoderManifestException("Start and End elements are not balanced");
			}
			if (num2 != 0)
			{
				throw new ApkDecoderManifestException("Start and End namespaces are not balanced");
			}
		}
	}
}