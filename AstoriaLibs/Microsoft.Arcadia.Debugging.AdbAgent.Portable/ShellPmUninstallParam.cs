using System;
using System.Text.RegularExpressions;

namespace Microsoft.Arcadia.Debugging.AdbAgent.Portable
{
	internal class ShellPmUninstallParam : ShellParamBase
	{
		private const string CommandName = "pm";

		private const string ShellUninstall = "uninstall";

		public string PackageName { get; private set; }

		public bool IsPackageNameSpecified => PackageName != null;

		private ShellPmUninstallParam(string[] tokens, bool fromInteractiveShell)
		{
			if (tokens.Length < 3)
			{
				throw new InvalidOperationException("Tokens must be of at least length 3.");
			}
			base.FromInteractiveShell = fromInteractiveShell;
			ProcessTokens(tokens);
		}

		public static ShellPmUninstallParam ParseFromInteractiveShell(string command)
		{
			return ParseUninstallParameters(command, isFromInteractiveShell: true);
		}

		public static ShellPmUninstallParam ParseFromOpen(string command)
		{
			return ParseUninstallParameters(command, isFromInteractiveShell: false);
		}

		private static ShellPmUninstallParam ParseUninstallParameters(string command, bool isFromInteractiveShell)
		{
			string[] array = StringParsingUtils.Tokenize(command);
			if (array.Length < 3)
			{
				return null;
			}
			if (!ShellParamBase.IsSystemCommand("pm", array[0], isFromInteractiveShell) || string.Compare(array[1], "uninstall", StringComparison.Ordinal) != 0)
			{
				return null;
			}
			return new ShellPmUninstallParam(array, isFromInteractiveShell);
		}

		private void ProcessTokens(string[] tokens)
		{
			Regex regex = new Regex("^([a-z0-9\\._]+)$", RegexOptions.IgnoreCase);
			Match match = regex.Match(tokens[2]);
			if (match.Success)
			{
				PackageName = match.Groups[1].Value;
			}
		}
	}
}