using System;
using System.Text.RegularExpressions;

namespace Microsoft.Arcadia.Debugging.AdbAgent.Portable
{
	internal class ShellAmStartParam : ShellParamBase
	{
		private const string DebuggingFlag = "-D";

		private const string DataFlag = "-d";

		private const string CategoryFlag = "-c";

		private const string ComponentFlag = "-n";

		private const string ActionFlag = "-a";

		private const string CommandName = "am";

		private const string AmStart = "start";

		private const string DefaultAction = "android.intent.action.MAIN";

		private const string DefaultCategory = "android.intent.category.LAUNCHER";

		public bool IsDebugging { get; private set; }

		public Intent Intent { get; private set; }

		public bool IntentPresent { get; private set; }

		private ShellAmStartParam(string[] tokens, bool isFromInteractiveShell)
		{
			if (tokens.Length < 3)
			{
				throw new InvalidOperationException("Tokens must be of at least length 3.");
			}
			Intent = new Intent();
			Intent.Action = "android.intent.action.MAIN";
			Intent.Category = "android.intent.category.LAUNCHER";
			base.FromInteractiveShell = isFromInteractiveShell;
			ProcessTokens(tokens);
		}

		public static ShellAmStartParam ParseFromOpen(string command)
		{
			return ParseInstallParameters(command, isFromInteractiveShell: false);
		}

		public static ShellAmStartParam ParseFromInteractiveShell(string command)
		{
			return ParseInstallParameters(command, isFromInteractiveShell: true);
		}

		private static ShellAmStartParam ParseInstallParameters(string command, bool isFromInteractiveShell)
		{
			string[] array = StringParsingUtils.Tokenize(command);
			if (array.Length < 3)
			{
				return null;
			}
			if (!ShellParamBase.IsSystemCommand("am", array[0], isFromInteractiveShell) || string.Compare(array[1], "start", StringComparison.Ordinal) != 0)
			{
				return null;
			}
			return new ShellAmStartParam(array, isFromInteractiveShell);
		}

		private void ProcessTokens(string[] tokens)
		{
			for (int i = 2; i < tokens.Length; i++)
			{
				string text = tokens[i];
				string text2 = ((i < tokens.Length - 1) ? tokens[i + 1] : null);
				if (!ParseExplicitComponent(text))
				{
					if (text == "-D")
					{
						IsDebugging = true;
					}
					else if (text2 != null && ParsePairedParameters(text, text2))
					{
						i++;
					}
				}
			}
		}

		private bool ParsePairedParameters(string leftToken, string rightToken)
		{
			switch (leftToken)
			{
				case "-n":
					return ParseExplicitComponent(rightToken);
				case "-a":
					Intent.Action = rightToken;
					break;
				case "-c":
					Intent.Category = rightToken;
					break;
				case "-d":
					return ParseDataUri(rightToken);
				default:
					return false;
			}
			return true;
		}

		private bool ParseDataUri(string uriToken)
		{
			if (Uri.TryCreate(uriToken, UriKind.RelativeOrAbsolute, out var result))
			{
				Intent.DataUri = result;
				return true;
			}
			return false;
		}

		private bool ParseExplicitComponent(string token)
		{
			Regex regex = new Regex("^([a-z0-9\\._]+)/([a-z0-9\\._]+)$", RegexOptions.IgnoreCase);
			Match match = regex.Match(token);
			if (match.Success)
			{
				Intent.PackageName = match.Groups[1].Value;
				Intent.ActivityName = match.Groups[2].Value;
				IntentPresent = true;
				return true;
			}
			return false;
		}
	}
}