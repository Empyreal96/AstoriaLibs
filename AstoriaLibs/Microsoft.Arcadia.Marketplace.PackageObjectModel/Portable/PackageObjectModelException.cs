using System;

namespace Microsoft.Arcadia.Marketplace.PackageObjectModel.Portable
{
	public class PackageObjectModelException : Exception
	{
		public PackageObjectModelException()
		{
		}

		public PackageObjectModelException(string message)
			: base(message)
		{
		}

		public PackageObjectModelException(string message, Exception inner)
			: base(message, inner)
		{
		}
	}
}