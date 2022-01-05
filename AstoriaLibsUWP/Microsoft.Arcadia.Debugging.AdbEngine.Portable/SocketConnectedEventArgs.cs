using System;

namespace Microsoft.Arcadia.Debugging.AdbEngine.Portable
{
	public sealed class SocketConnectedEventArgs : EventArgs
	{
		public ISocket SocketConnected { get; private set; }

		public SocketConnectedEventArgs(ISocket socketConnected)
		{
			SocketConnected = socketConnected;
		}
	}
}
