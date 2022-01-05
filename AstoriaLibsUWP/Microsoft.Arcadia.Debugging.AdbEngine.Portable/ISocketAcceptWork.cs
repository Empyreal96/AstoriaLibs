using System;

namespace Microsoft.Arcadia.Debugging.AdbEngine.Portable
{
	public interface ISocketAcceptWork : IWork, IDisposable
	{
		event EventHandler ListenStarted;

		event EventHandler<SocketAcceptedEventArgs> SocketAccepted;
	}
}
