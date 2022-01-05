using System;
using Microsoft.Arcadia.Marketplace.PackageObjectModel.Portable;

namespace Microsoft.Arcadia.Marketplace.Decoder.Portable
{
	public class ApkDecoderManifestException : ApkFormatException
	{
		public ApkDecoderManifestException()
		{
		}

		public ApkDecoderManifestException(string message)
			: base(message)
		{
		}

		public ApkDecoderManifestException(string message, Exception inner)
			: base(message, inner)
		{
		}
	}
}