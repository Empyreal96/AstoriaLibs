using System;

namespace Microsoft.Arcadia.Debugging.AdbAgent.Portable
{
	public class PackageDeploymentResult
	{
		public Exception Error { get; private set; }

		public Exception ExtendedError { get; private set; }

		public PackageDeploymentResult(Exception error, Exception extendedError)
		{
			Error = error;
			ExtendedError = extendedError;
		}
	}
}