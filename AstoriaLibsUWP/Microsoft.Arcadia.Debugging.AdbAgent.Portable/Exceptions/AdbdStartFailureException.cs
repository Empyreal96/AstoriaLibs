using System;

namespace Microsoft.Arcadia.Debugging.AdbAgent.Portable.Exceptions
{
	public class AdbdStartFailureException : Exception
	{
		public AdbdStartFailureException()
			: base("Failure starting ADBD.")
		{
		}

		public AdbdStartFailureException(string message)
			: base(message)
		{
		}

		public AdbdStartFailureException(string message, Exception innerException)
			: base(message, innerException)
		{
		}

		public AdbdStartFailureException(Exception ex)
			: base("Failure starting ADBD.", ex)
		{
		}
	}
}