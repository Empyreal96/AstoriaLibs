using System;
using Microsoft.Arcadia.Debugging.AdbProtocol.Portable;

namespace Microsoft.Arcadia.Debugging.AdbEngine.Portable
{
	public sealed class AdbPacketReceivedEventArgs : EventArgs
	{
		public AdbPacket Packet { get; private set; }

		public AdbPacketReceivedEventArgs(AdbPacket packet)
		{
			Packet = packet;
		}
	}
}
