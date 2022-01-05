using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using Microsoft.Arcadia.Marketplace.Utils.Log;
using Microsoft.Arcadia.Marketplace.Utils.Parsers;
using Microsoft.Arcadia.Marketplace.Utils.Portable;

namespace Microsoft.Arcadia.Marketplace.Converter
{
	[SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "Tool reports error on some abbreviations such as en-US")]
	public sealed class StringsWriter
	{
		private const string StringsFolderName = "Strings";

		private const string ReswFileName = "Resources.resw";

		private string stringsFolderPath;

		private Dictionary<string, Dictionary<string, string>> strings;

		public IReadOnlyCollection<string> AllLanguageQualifiers
		{
			get
			{
				List<string> list = new List<string>();
				foreach (KeyValuePair<string, Dictionary<string, string>> @string in strings)
				{
					list.Add(@string.Key);
				}
				return list;
			}
		}

		public StringsWriter(string rootFolderPath)
		{
			if (string.IsNullOrWhiteSpace(rootFolderPath))
			{
				throw new ArgumentException("Folder path is null or empty", "rootFolderPath");
			}
			LoggerCore.Log("Creating StringsWriter, root folder = " + rootFolderPath);
			stringsFolderPath = Path.Combine(new string[2] { rootFolderPath, "Strings" });
			strings = new Dictionary<string, Dictionary<string, string>>(StringComparer.OrdinalIgnoreCase);
		}

		public void AddString(string name, string value, string languageQualifier)
		{
			if (string.IsNullOrWhiteSpace(name))
			{
				throw new ArgumentException("Resource name is null or empty", "name");
			}
			if (string.IsNullOrWhiteSpace(value))
			{
				throw new ArgumentException("Resource value is null or empty", "value");
			}
			string text = ConvertLanguageQualifierForWindows(languageQualifier);
			if (!LanguageQualifier.IsValidLanguageQualifier(text))
			{
				LoggerCore.Log("Invalid language qualifier: {0} or not supported by Windows.", languageQualifier);
				return;
			}
			Dictionary<string, string> value2 = null;
			if (!strings.TryGetValue(text, out value2))
			{
				LoggerCore.Log("Creating ResXResourceWriter for " + text);
				value2 = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
				strings[text] = value2;
			}
			LoggerCore.Log("Adding string, name = {0}, value = {1}, languageQualifier = {2}", name, value, text);
			value2[name] = value;
		}

		public void WriteReswFiles()
		{
			LoggerCore.Log("Writing RESW files");
			IPortableResourceUtils resourceUtils = PortableUtilsServiceLocator.ResourceUtils;
			foreach (KeyValuePair<string, Dictionary<string, string>> @string in strings)
			{
				string reswFilePath = GetReswFilePath(stringsFolderPath, @string.Key);
				LoggerCore.Log("Writing " + reswFilePath);
				resourceUtils.WriteNewResX(reswFilePath, @string.Value);
			}
			LoggerCore.Log("Finished writing RESW files");
		}

		public void CleanupAllReswFiles()
		{
			if (PortableUtilsServiceLocator.FileUtils.DirectoryExists(stringsFolderPath))
			{
				PortableUtilsServiceLocator.FileUtils.DeleteDirectory(stringsFolderPath);
			}
		}

		private static string GetReswFolderPathAndEnsureExisting(string stringsPath, string languageQualifier)
		{
			string text = Path.Combine(new string[2] { stringsPath, languageQualifier });
			if (!PortableUtilsServiceLocator.FileUtils.FileExists(text))
			{
				PortableUtilsServiceLocator.FileUtils.CreateDirectory(text);
			}
			return text;
		}

		private static string GetReswFilePath(string stringsPath, string languageQualifier)
		{
			return Path.Combine(new string[2]
			{
			GetReswFolderPathAndEnsureExisting(stringsPath, languageQualifier),
			"Resources.resw"
			});
		}

		private static string ConvertLanguageQualifierForWindows(string languageQualifier)
		{
			if (string.Compare(languageQualifier, "zh", StringComparison.OrdinalIgnoreCase) == 0)
			{
				languageQualifier = "zh-Hans";
			}
			if (string.Compare(languageQualifier, "any", StringComparison.OrdinalIgnoreCase) == 0 || string.IsNullOrWhiteSpace(languageQualifier))
			{
				languageQualifier = "en-US";
			}
			return languageQualifier;
		}
	}
}