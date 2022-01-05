using System;
using Microsoft.Arcadia.Marketplace.PackageObjectModel.Portable;

namespace Microsoft.Arcadia.Marketplace.Decoder.Portable
{
	public class ApkDecoderCommonException : ApkFormatException
	{
		public ApkDecoderCommonException()
		{
		}

		public ApkDecoderCommonException(string message)
			: base(message)
		{
		}

		public ApkDecoderCommonException(string message, Exception inner)
			: base(message, inner)
		{
		}
	}
}