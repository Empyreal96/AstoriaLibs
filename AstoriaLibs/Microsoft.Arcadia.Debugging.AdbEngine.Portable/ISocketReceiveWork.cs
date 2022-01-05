using System;

namespace Microsoft.Arcadia.Debugging.AdbEngine.Portable
{
	public interface ISocketReceiveWork : IWork, IDisposable
	{
		event EventHandler<SocketDataReceivedEventArgs> DataReceived;
	}
}
