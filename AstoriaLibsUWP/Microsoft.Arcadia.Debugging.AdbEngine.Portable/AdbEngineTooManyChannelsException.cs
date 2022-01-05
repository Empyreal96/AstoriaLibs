using System;

namespace Microsoft.Arcadia.Debugging.AdbEngine.Portable
{
	public class AdbEngineTooManyChannelsException : AdbEngineException
	{
		public AdbEngineTooManyChannelsException()
		{
		}

		public AdbEngineTooManyChannelsException(string message)
			: base(message)
		{
		}

		public AdbEngineTooManyChannelsException(string message, Exception inner)
			: base(message, inner)
		{
		}
	}
}
