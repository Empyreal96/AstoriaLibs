using System;

namespace Microsoft.Arcadia.Marketplace.PackageObjectModel.Portable
{
	public class ApkFormatException : Exception
	{
		public ApkFormatException()
		{
		}

		public ApkFormatException(string message)
			: base(message)
		{
		}

		public ApkFormatException(string message, Exception inner)
			: base(message, inner)
		{
		}
	}
}