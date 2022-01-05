using System;
using System.Collections.Generic;

namespace Microsoft.Arcadia.Debugging.AdbAgent.Portable
{
	internal class ShellPmInstallParam : ShellParamBase
	{
		private const string CommandName = "pm";

		private const string ShellInstall = "install";

		public IReadOnlyCollection<string> Options { get; private set; }

		public string ApkFilePath { get; private set; }

		private ShellPmInstallParam(List<string> options, string apkFilePath, bool fromInteractiveShell)
		{
			Options = options;
			ApkFilePath = apkFilePath;
			base.FromInteractiveShell = fromInteractiveShell;
		}

		public static ShellPmInstallParam ParseFromInteractiveShell(string command)
		{
			return ParseInstallParameters(command, isFromInteractiveShell: true);
		}

		public static ShellPmInstallParam ParseFromOpen(string command)
		{
			return ParseInstallParameters(command, isFromInteractiveShell: false);
		}

		private static ShellPmInstallParam ParseInstallParameters(string command, bool isFromInteractiveShell)
		{
			string[] array = StringParsingUtils.Tokenize(command);
			if (array.Length < 3)
			{
				return null;
			}
			if (!ShellParamBase.IsSystemCommand("pm", array[0], isFromInteractiveShell) || string.Compare(array[1], "install", StringComparison.Ordinal) != 0)
			{
				return null;
			}
			List<string> list = null;
			string text = null;
			for (int i = 2; i < array.Length; i++)
			{
				string text2 = array[i];
				if (text2[0] == '-')
				{
					if (list == null)
					{
						list = new List<string>();
					}
					list.Add(text2);
				}
				else if (string.IsNullOrEmpty(text))
				{
					text = text2;
					break;
				}
			}
			return new ShellPmInstallParam(list, text, isFromInteractiveShell);
		}
	}
}