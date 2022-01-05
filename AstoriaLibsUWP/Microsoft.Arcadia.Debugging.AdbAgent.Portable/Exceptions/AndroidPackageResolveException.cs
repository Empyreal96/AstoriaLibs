using System;

namespace Microsoft.Arcadia.Debugging.AdbAgent.Portable.Exceptions
{
	public class AndroidPackageResolveException : Exception
	{
		public AndroidPackageResolveException()
			: base("Error resolving Android Package.")
		{
		}

		public AndroidPackageResolveException(string message)
			: base(message)
		{
		}

		public AndroidPackageResolveException(string message, Exception innerException)
			: base(message, innerException)
		{
		}

		public AndroidPackageResolveException(Exception ex)
			: base("Error resolving Android Package.", ex)
		{
		}
	}
}