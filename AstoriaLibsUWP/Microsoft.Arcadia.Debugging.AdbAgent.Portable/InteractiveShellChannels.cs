using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Microsoft.Arcadia.Marketplace.Utils.Log;

namespace Microsoft.Arcadia.Debugging.AdbAgent.Portable
{
	internal static class InteractiveShellChannels
	{
		private class InteractiveShell
		{
			public uint AdbServerChannelId { get; set; }

			public uint AdbdChannelId { get; set; }

			public bool IsFullyOpened { get; set; }

			public override string ToString()
			{
				return string.Format(CultureInfo.CurrentCulture, "ADB Server Channel ID: {0}. ADBD Channel ID: {1}.", new object[2] { AdbServerChannelId, AdbdChannelId });
			}
		}

		private static List<InteractiveShell> interactiveShells = new List<InteractiveShell>();

		private static object lockObj = new object();

		public static bool ChannelExists(uint adbServerChannelId, uint adbdChannelId)
		{
			lock (lockObj)
			{
				if (interactiveShells.Count == 0)
				{
					return false;
				}
				return interactiveShells.Any((InteractiveShell m) => m.AdbdChannelId == adbdChannelId && m.AdbServerChannelId == adbServerChannelId);
			}
		}

		public static void AdbServerOpen(uint adbServerId)
		{
			lock (lockObj)
			{
				if (adbServerId == 0)
				{
					LoggerCore.Log("ADB Server channel identifier is 0. Ignoring.");
					return;
				}
				if (interactiveShells.Where((InteractiveShell m) => m.AdbServerChannelId == adbServerId).Count() > 0)
				{
					LoggerCore.Log(LoggerCore.LogLevels.Warning, "ADB Server Channel ID {0} already exists. Ignoring.", adbServerId);
					return;
				}
				InteractiveShell interactiveShell = new InteractiveShell();
				interactiveShell.AdbServerChannelId = adbServerId;
				InteractiveShell item = interactiveShell;
				interactiveShells.Add(item);
				LoggerCore.Log("Created new pending open interactive shell entry. ADB Server Channel ID: {0}.", adbServerId);
			}
		}

		public static void AdbdOpened(uint adbServerId, uint adbdId)
		{
			lock (lockObj)
			{
				if (adbServerId == 0)
				{
					LoggerCore.Log("ADB Server channel identifier specified was 0. Ignoring.");
				}
				else if (adbdId == 0)
				{
					LoggerCore.Log(LoggerCore.LogLevels.Warning, "ADBD channel identifier specified was 0. Ignoring.");
				}
				else
				{
					if (interactiveShells.Count == 0)
					{
						return;
					}
					InteractiveShell interactiveShell = interactiveShells.Where((InteractiveShell m) => m.AdbServerChannelId == adbServerId).FirstOrDefault();
					if (interactiveShell != null)
					{
						if (interactiveShell.IsFullyOpened)
						{
							LoggerCore.Log("{0} already opened. Ignoring.", interactiveShell);
							return;
						}
						if (interactiveShell.AdbdChannelId == adbdId)
						{
							LoggerCore.Log(LoggerCore.LogLevels.Warning, "ADBD channel identifier already exists. Ignoring.");
							return;
						}
						interactiveShell.AdbdChannelId = adbdId;
						interactiveShell.IsFullyOpened = true;
						LoggerCore.Log("New fully constructed interactive shell: {0}.", interactiveShell);
					}
				}
			}
		}

		public static void AdbServerChannelClose(uint adbdChannelId)
		{
			lock (lockObj)
			{
				if (interactiveShells.Count != 0 && interactiveShells.RemoveAll((InteractiveShell m) => m.AdbdChannelId == adbdChannelId) == 0)
				{
					LoggerCore.Log("Interactive shell successfully closed by ADB Server.");
				}
			}
		}

		public static void AdbdChannelClose(uint adbServerChannelId)
		{
			lock (lockObj)
			{
				if (interactiveShells.Count != 0)
				{
					int num = interactiveShells.RemoveAll((InteractiveShell m) => m.AdbServerChannelId == adbServerChannelId);
					if (num > 0)
					{
						LoggerCore.Log("Interactive shell successfully closed by ADBD.");
					}
				}
			}
		}
	}
}