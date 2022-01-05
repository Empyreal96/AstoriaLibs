using System;

namespace Microsoft.Arcadia.Marketplace.PackageObjectModel.Portable
{
	public class DecoderManifestNoApplicationException : ApkFormatException
	{
		public DecoderManifestNoApplicationException()
		{
		}

		public DecoderManifestNoApplicationException(string message)
			: base(message)
		{
		}

		public DecoderManifestNoApplicationException(string message, Exception inner)
			: base(message, inner)
		{
		}
	}
}