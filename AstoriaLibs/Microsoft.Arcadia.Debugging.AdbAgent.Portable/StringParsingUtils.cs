using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;

namespace Microsoft.Arcadia.Debugging.AdbAgent.Portable
{
	internal static class StringParsingUtils
	{
		[SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "StyleCop reports incorrect spell in the example section")]
		public static string[] Tokenize(string input)
		{
			List<string> list = new List<string>();
			int num = 0;
			while (num < input.Length)
			{
				int num2 = IndexOfNonWhitespace(input, num);
				if (num2 < 0)
				{
					break;
				}
				int num3 = -1;
				if (input[num2] == '\'' || input[num2] == '"')
				{
					num3 = input.IndexOf(input[num2], num2 + 1);
					num3 = ((num3 < 0) ? input.Length : (num3 + 1));
				}
				else
				{
					num3 = IndexOfWhitespaceOrQuota(input, num2 + 1);
					if (num3 < 0)
					{
						num3 = input.Length;
					}
				}
				string text = input.Substring(num2, num3 - num2);
				text = text.Trim('\'', '"');
				text = text.Trim();
				text = text.Replace("\\ ", " ");
				text = text.Replace("\\(", "(");
				text = text.Replace("\\)", ")");
				if (!string.IsNullOrEmpty(text) && !string.IsNullOrWhiteSpace(text))
				{
					list.Add(text);
				}
				num = num3;
			}
			return list.ToArray();
		}

		private static int IndexOfWhitespaceOrQuota(string input, int startAt)
		{
			Regex regex = new Regex("\"|'|(?<!\\\\)\\s");
			Match match = regex.Match(input, startAt);
			if (!match.Success)
			{
				return -1;
			}
			return match.Index;
		}

		private static int IndexOfNonWhitespace(string input, int startAt)
		{
			Regex regex = new Regex("\\S");
			Match match = regex.Match(input, startAt);
			if (!match.Success)
			{
				return -1;
			}
			return match.Index;
		}
	}
}