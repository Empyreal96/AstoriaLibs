using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Microsoft.Arcadia.Marketplace.Decoder.Portable.Common;
using Microsoft.Arcadia.Marketplace.Decoder.Portable.Manifest.Types;
using Microsoft.Arcadia.Marketplace.Utils.Log;

namespace Microsoft.Arcadia.Marketplace.Decoder.Portable.Manifest.Decoder
{
	public sealed class XmlDecoder : StreamDecoder
	{
		private XmlChunk xmlChunk;

		private string stringContent;

		public XmlDecoder(string apkManifestFilePath)
			: base(apkManifestFilePath)
		{
			if (string.IsNullOrWhiteSpace(apkManifestFilePath))
			{
				throw new ArgumentException("APK Manifest file path must be provided", "apkManifestFilePath");
			}
			LoggerCore.Log("Decoding APK Manifest file: {0}", apkManifestFilePath);
		}

		public async Task<string> RetrieveStringContentAsync()
		{
			return await Task.Run(delegate
			{
				if (string.IsNullOrWhiteSpace(stringContent))
				{
					LoggerCore.Log("Decoding Manifest XML Chunk as string content");
					XmlChunk xmlChunk = GetXmlChunk();
					stringContent = XmlChunkDecoder.Decode(xmlChunk);
				}
				return stringContent;
			}).ConfigureAwait(continueOnCapturedContext: false);
		}

		public bool IsValidAndroidEncodedXmlFile()
		{
			uint num = base.Offset;
			base.Offset = 0u;
			ushort num2 = PeakUint16();
			base.Offset = num;
			LoggerCore.Log("Chunk Type: {0} ({1})", (ChunkType)num2, num2);
			if (num2 == 3 || num2 == 1)
			{
				return true;
			}
			return false;
		}

		[SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "Microsoft.Arcadia.Marketplace.Utils.Log.LoggerCore.Log(System.String)", Justification = "Internal logs")]
		internal XmlChunk GetXmlChunk()
		{
			if (xmlChunk == null)
			{
				LoggerCore.Log("Decoding Manifest raw stream data as XML Chunk.");
				xmlChunk = ChunkDecoder.Decode(this) as XmlChunk;
			}
			return xmlChunk;
		}
	}
}