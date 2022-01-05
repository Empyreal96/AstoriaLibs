using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.Arcadia.Debugging.AdbAgent.Portable
{
	[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "Reviewed.")]
	internal class ShellRmParams : ShellParamBase
	{
		private const string ShellRmPrefix = "shell:rm";

		public string FilePath { get; private set; }

		public IReadOnlyCollection<string> Options { get; private set; }

		private ShellRmParams(List<string> options, string filePath)
		{
			FilePath = filePath;
			Options = options;
		}

		public static ShellRmParams Parse(string command)
		{
			string[] array = StringParsingUtils.Tokenize(command);
			if (array.Length < 2 || string.Compare(array[0], "shell:rm", StringComparison.OrdinalIgnoreCase) != 0)
			{
				return null;
			}
			List<string> list = null;
			string filePath = null;
			for (int i = 1; i < array.Length; i++)
			{
				string text = array[i];
				if (text[0] == '-')
				{
					if (list == null)
					{
						list = new List<string>();
					}
					list.Add(text);
					continue;
				}
				filePath = text;
				break;
			}
			return new ShellRmParams(list, filePath);
		}
	}
}