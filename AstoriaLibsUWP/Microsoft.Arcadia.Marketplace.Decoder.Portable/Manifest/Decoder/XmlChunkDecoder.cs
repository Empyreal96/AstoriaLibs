using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Text;
using Microsoft.Arcadia.Marketplace.Decoder.Portable.Common;
using Microsoft.Arcadia.Marketplace.Decoder.Portable.Manifest.Types;
using Microsoft.Arcadia.Marketplace.Utils.Log;

namespace Microsoft.Arcadia.Marketplace.Decoder.Portable.Manifest.Decoder
{
	internal sealed class XmlChunkDecoder
	{
		[SuppressMessage("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily", Justification = "Need to cast namespace chunk")]
		public static string Decode(XmlChunk xmlChunk)
		{
			LoggerCore.Log("Decoding XML Chunk");
			StringBuilder stringBuilder = new StringBuilder("<?xml version=\"1.0\" encoding=\"utf-8\" standalone=\"no\"?>\r\n");
			XmlDataDecoder xmlDataDecoder = new XmlDataDecoder(xmlChunk.ChunkStringPool.Strings, xmlChunk.XmlResourceMapChunk.ResourceIds);
			foreach (XmlItemChunk xmlItemChunk in xmlChunk.XmlItemChunkList)
			{
				switch (xmlItemChunk.ChunkType)
				{
					case ChunkType.ResXmlFirstChunkType:
						{
							XmlNamespaceChunk xmlNamespaceChunk2 = xmlItemChunk as XmlNamespaceChunk;
							if (xmlDataDecoder.XmlnsUriToPrefix.ContainsKey(xmlNamespaceChunk2.Uri))
							{
								XmlNamespaceMapItem xmlNamespaceMapItem2 = xmlDataDecoder.XmlnsUriToPrefix[xmlNamespaceChunk2.Uri];
								if (xmlNamespaceMapItem2.Prefix != xmlNamespaceChunk2.Prefix)
								{
									LoggerCore.Log("Multiple prefixes point to same namespace uri.");
								}
								xmlNamespaceMapItem2.Count++;
							}
							else
							{
								XmlNamespaceMapItem value3 = new XmlNamespaceMapItem(xmlNamespaceChunk2.Prefix);
								xmlDataDecoder.XmlnsUriToPrefix.Add(xmlNamespaceChunk2.Uri, value3);
							}
							xmlDataDecoder.XmlnsShow.Add(xmlNamespaceChunk2.Prefix, xmlNamespaceChunk2.Uri);
							break;
						}
					case ChunkType.ResXmlEndNamespaceType:
						{
							XmlNamespaceChunk xmlNamespaceChunk = xmlItemChunk as XmlNamespaceChunk;
							XmlNamespaceMapItem xmlNamespaceMapItem = xmlDataDecoder.XmlnsUriToPrefix[xmlNamespaceChunk.Uri];
							xmlNamespaceMapItem.Count--;
							if (xmlNamespaceMapItem.Count == 0)
							{
								xmlDataDecoder.XmlnsUriToPrefix.Remove(xmlNamespaceChunk.Uri);
							}
							xmlDataDecoder.XmlnsShow.Remove(xmlNamespaceChunk.Prefix);
							break;
						}
					case ChunkType.ResXmlStartElementType:
						{
							XmlStartElementChunk xmlStartElementChunk = xmlItemChunk as XmlStartElementChunk;
							XmlStartElementChunkDecoder value2 = new XmlStartElementChunkDecoder(xmlStartElementChunk, xmlDataDecoder);
							stringBuilder.Append(value2);
							xmlDataDecoder.IndentCount++;
							break;
						}
					case ChunkType.ResXmlEndElementType:
						{
							XmlEndElementChunk xmlEndElementChunk = xmlItemChunk as XmlEndElementChunk;
							XmlEndElementChunkDecoder value = new XmlEndElementChunkDecoder(xmlEndElementChunk, xmlDataDecoder);
							xmlDataDecoder.IndentCount--;
							stringBuilder.Append(value);
							break;
						}
					case ChunkType.ResXmlCDataType:
						stringBuilder.AppendFormat(CultureInfo.InvariantCulture, "{0} [CDATA here...]", new object[1] { xmlDataDecoder.IndentString });
						break;
					default:
						throw new ApkDecoderManifestException("Unexpected XML Item Chunk type" + xmlItemChunk.ChunkType);
				}
			}
			return stringBuilder.ToString();
		}
	}
}