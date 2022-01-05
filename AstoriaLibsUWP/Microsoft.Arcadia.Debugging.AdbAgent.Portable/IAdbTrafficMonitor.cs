using Microsoft.Arcadia.Debugging.AdbProtocol.Portable;

namespace Microsoft.Arcadia.Debugging.AdbAgent.Portable
{
	public interface IAdbTrafficMonitor
	{
		void OnAdbTraffic(AdbPacket adbPacket, AdbTrafficDirection direction);
	}
}