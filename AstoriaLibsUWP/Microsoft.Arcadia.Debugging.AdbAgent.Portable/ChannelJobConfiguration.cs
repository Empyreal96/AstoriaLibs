using Microsoft.Arcadia.Debugging.AdbEngine.Portable;

namespace Microsoft.Arcadia.Debugging.AdbAgent.Portable
{
	internal class ChannelJobConfiguration
	{
		public uint LocalId { get; set; }

		public uint RemoteId { get; set; }

		public IAdbChannelClientManager RemoteChannelManager { get; set; }

		public AdbPacketSendWork AdbServerSender { get; set; }
	}
}