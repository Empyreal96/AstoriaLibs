using System;
using Microsoft.Arcadia.Marketplace.PackageObjectModel.Portable;

namespace Microsoft.Arcadia.Marketplace.Decoder.Portable
{
	public class ApkDecoderResourcesException : ApkFormatException
	{
		public ApkDecoderResourcesException()
		{
		}

		public ApkDecoderResourcesException(string message)
			: base(message)
		{
		}

		public ApkDecoderResourcesException(string message, Exception inner)
			: base(message, inner)
		{
		}
	}
}