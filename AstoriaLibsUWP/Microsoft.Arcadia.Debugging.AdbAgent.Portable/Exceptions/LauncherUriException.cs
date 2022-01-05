using System;

namespace Microsoft.Arcadia.Debugging.AdbAgent.Portable.Exceptions
{
	public class LauncherUriException : Exception
	{
		public LauncherUriException()
			: base("Error launching the URI.")
		{
		}

		public LauncherUriException(string message)
			: base(message)
		{
		}

		public LauncherUriException(string message, Exception innerException)
			: base(message, innerException)
		{
		}

		public LauncherUriException(Exception ex)
			: base("Error launching the URI.", ex)
		{
		}
	}
}