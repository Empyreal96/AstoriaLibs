using System;

namespace Microsoft.Arcadia.Debugging.AdbAgent.Portable
{
	public class ShellParamBase
	{
		private const string SystemBinAbsoluteRemotePath = "/system/bin";

		private const string InlineShellPrefix = "shell:";

		public bool FromInteractiveShell { get; protected set; }

		protected ShellParamBase()
		{
		}

		public static bool IsSystemCommand(string expectedSystemCommand, string candidateShellCommand, bool fromInteractiveShell)
		{
			if (string.IsNullOrEmpty(expectedSystemCommand))
			{
				throw new ArgumentException("expectedSystemCommand cannot be null or empty.", "expectedSystemCommand");
			}
			if (string.IsNullOrEmpty(candidateShellCommand))
			{
				return false;
			}
			string text = "/system/bin" + "/" + expectedSystemCommand;
			if (fromInteractiveShell)
			{
				if (string.CompareOrdinal(text, candidateShellCommand) != 0)
				{
					return string.CompareOrdinal(expectedSystemCommand, candidateShellCommand) == 0;
				}
				return true;
			}
			string strA = "shell:" + text;
			string strA2 = "shell:" + expectedSystemCommand;
			if (string.CompareOrdinal(strA, candidateShellCommand) != 0)
			{
				return string.CompareOrdinal(strA2, candidateShellCommand) == 0;
			}
			return true;
		}
	}
}