using System;
using Microsoft.Arcadia.Debugging.AdbEngine.Portable;
using Windows.Networking.Sockets;

namespace Microsoft.Arcadia.Debugging.AdbEngine.Mobile
{
	internal class SocketImpl : ISocket
	{
		public StreamSocket RealSocket { get; private set; }

		public SocketImpl(StreamSocket realSocket)
		{
			if (realSocket == null)
			{
				throw new ArgumentNullException("realSocket");
			}
			RealSocket = realSocket;
		}

		void ISocket.Close()
		{
			RealSocket.Dispose();
		}
	}
}