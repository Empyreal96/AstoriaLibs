using System;

namespace Microsoft.Arcadia.Marketplace.Decoder.Portable
{
	public class ApkDecoderConfigException : Exception
	{
		public ApkDecoderConfigException()
		{
		}

		public ApkDecoderConfigException(string message)
			: base(message)
		{
		}

		public ApkDecoderConfigException(string message, Exception inner)
			: base(message, inner)
		{
		}
	}
}