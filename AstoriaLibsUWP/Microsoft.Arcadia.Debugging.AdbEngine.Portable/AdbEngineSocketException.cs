using System;

namespace Microsoft.Arcadia.Debugging.AdbEngine.Portable
{
	public class AdbEngineSocketException : AdbEngineException
	{
		public AdbEngineSocketException()
		{
		}

		public AdbEngineSocketException(string message)
			: base(message)
		{
		}

		public AdbEngineSocketException(string message, Exception inner)
			: base(message, inner)
		{
		}
	}
}
