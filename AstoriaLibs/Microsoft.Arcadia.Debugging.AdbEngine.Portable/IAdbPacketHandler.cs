using Microsoft.Arcadia.Debugging.AdbProtocol.Portable;

namespace Microsoft.Arcadia.Debugging.AdbEngine.Portable
{
	public interface IAdbPacketHandler
	{
		bool HandlePacket(AdbPacket receivedPacket);
	}
}
