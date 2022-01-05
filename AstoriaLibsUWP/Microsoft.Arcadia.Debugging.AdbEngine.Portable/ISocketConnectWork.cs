using System;

namespace Microsoft.Arcadia.Debugging.AdbEngine.Portable
{
	public interface ISocketConnectWork : IWork, IDisposable
	{
		event EventHandler<SocketConnectedEventArgs> SocketConnected;
	}
}
