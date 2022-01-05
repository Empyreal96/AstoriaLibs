using System;

namespace Microsoft.Arcadia.Debugging.AdbEngine.Portable
{
	public class AdbEngineException : Exception
	{
		public AdbEngineException()
		{
		}

		public AdbEngineException(string message)
			: base(message)
		{
		}

		public AdbEngineException(string message, Exception inner)
			: base(message, inner)
		{
		}
	}
}
