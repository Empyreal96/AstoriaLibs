using System;

namespace Microsoft.Arcadia.Debugging.AdbAgent.Portable.Exceptions
{
	public class ProjectAStartFailureException : Exception
	{
		public ProjectAStartFailureException()
			: base("Failure starting ProjectA.")
		{
		}

		public ProjectAStartFailureException(string message)
			: base(message)
		{
		}

		public ProjectAStartFailureException(string message, Exception innerException)
			: base(message, innerException)
		{
		}

		public ProjectAStartFailureException(Exception ex)
			: base("Failure starting ProjectA.", ex)
		{
		}
	}
}