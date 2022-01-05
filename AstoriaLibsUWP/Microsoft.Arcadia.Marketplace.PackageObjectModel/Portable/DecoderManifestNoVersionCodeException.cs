using System;

namespace Microsoft.Arcadia.Marketplace.PackageObjectModel.Portable
{
	public class DecoderManifestNoVersionCodeException : ApkFormatException
	{
		public DecoderManifestNoVersionCodeException()
		{
		}

		public DecoderManifestNoVersionCodeException(string message)
			: base(message)
		{
		}

		public DecoderManifestNoVersionCodeException(string message, Exception inner)
			: base(message, inner)
		{
		}
	}
}