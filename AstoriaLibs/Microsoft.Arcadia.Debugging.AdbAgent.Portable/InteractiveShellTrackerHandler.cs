using System;
using Microsoft.Arcadia.Debugging.AdbEngine.Portable;
using Microsoft.Arcadia.Debugging.AdbProtocol.Portable;

namespace Microsoft.Arcadia.Debugging.AdbAgent.Portable
{
	internal class InteractiveShellTrackerHandler : IAdbPacketHandler
	{
		private const string ShellOpenCommand = "shell:";

		private bool directionIsFromAdbd;

		public InteractiveShellTrackerHandler(bool directionIsFromAdbd)
		{
			this.directionIsFromAdbd = directionIsFromAdbd;
		}

		bool IAdbPacketHandler.HandlePacket(AdbPacket receivedPacket)
		{
			if (receivedPacket == null)
			{
				throw new ArgumentNullException("receivedPacket");
			}
			if (directionIsFromAdbd && receivedPacket.Command == 1497451343)
			{
				InteractiveShellChannels.AdbdOpened(receivedPacket.Arg1, receivedPacket.Arg0);
			}
			else if (!directionIsFromAdbd && receivedPacket.Command == 1313165391)
			{
				string text = AdbPacket.ParseStringFromData(receivedPacket.Data);
				if (text == null)
				{
					return false;
				}
				string[] array = StringParsingUtils.Tokenize(text);
				if (array.Length > 0 && string.Compare(array[0], "shell:", StringComparison.OrdinalIgnoreCase) == 0)
				{
					InteractiveShellChannels.AdbServerOpen(receivedPacket.Arg0);
				}
			}
			else if (receivedPacket.Command == 1163086915)
			{
				if (directionIsFromAdbd)
				{
					InteractiveShellChannels.AdbdChannelClose(receivedPacket.Arg1);
				}
				else
				{
					InteractiveShellChannels.AdbServerChannelClose(receivedPacket.Arg1);
				}
			}
			return false;
		}
	}
}