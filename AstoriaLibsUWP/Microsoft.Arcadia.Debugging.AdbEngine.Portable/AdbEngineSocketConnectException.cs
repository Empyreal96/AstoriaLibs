using System;

namespace Microsoft.Arcadia.Debugging.AdbEngine.Portable
{
	public class AdbEngineSocketConnectException : AdbEngineSocketException
	{
		public AdbEngineSocketConnectException()
		{
		}

		public AdbEngineSocketConnectException(string message)
			: base(message)
		{
		}

		public AdbEngineSocketConnectException(string message, Exception inner)
			: base(message, inner)
		{
		}
	}
}
