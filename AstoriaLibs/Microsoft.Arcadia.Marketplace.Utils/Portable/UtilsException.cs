using System;

namespace Microsoft.Arcadia.Marketplace.Utils.Portable
{
	public class UtilsException : Exception
	{
		public UtilsException()
		{
		}

		public UtilsException(string message)
			: base(message)
		{
		}

		public UtilsException(string message, Exception inner)
			: base(message, inner)
		{
		}
	}
}