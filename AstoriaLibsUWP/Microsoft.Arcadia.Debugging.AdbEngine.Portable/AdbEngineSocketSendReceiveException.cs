using System;

namespace Microsoft.Arcadia.Debugging.AdbEngine.Portable
{
	public class AdbEngineSocketSendReceiveException : AdbEngineSocketException
	{
		public string SocketIdentifier { get; private set; }

		public string Reason { get; private set; }

		public AdbEngineSocketSendReceiveException()
		{
		}

		public AdbEngineSocketSendReceiveException(string message)
			: base(message)
		{
		}

		public AdbEngineSocketSendReceiveException(string socketIdentifier, string reason)
		{
			SocketIdentifier = socketIdentifier;
			Reason = reason;
		}

		public AdbEngineSocketSendReceiveException(string message, Exception inner)
			: base(message, inner)
		{
		}
	}
}
