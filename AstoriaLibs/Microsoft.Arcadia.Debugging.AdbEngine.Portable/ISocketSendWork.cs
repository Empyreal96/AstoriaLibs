using System;

namespace Microsoft.Arcadia.Debugging.AdbEngine.Portable
{
	public interface ISocketSendWork : IWork, IDisposable
	{
		void EnqueueForSend(byte[] data);
	}
}
