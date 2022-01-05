using System;

namespace Microsoft.Arcadia.Debugging.AdbEngine.Portable
{
	public sealed class SocketAcceptedEventArgs : EventArgs
	{
		public ISocket SocketAccepted { get; private set; }

		public SocketAcceptedEventArgs(ISocket socketAccepted)
		{
			SocketAccepted = socketAccepted;
		}
	}
}
