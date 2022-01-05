using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Arcadia.Marketplace.Utils.Log;
using Windows.Data.Xml.Dom;
using Windows.Storage;

namespace Microsoft.Arcadia.Marketplace.Utils.Portable
{
	public sealed class XmlDocWriter
	{
		private XmlDocument xmlDoc;

		private string defaultNamespaceUri;

		private string defaultNamespacePrefix;

		private IDictionary<string, string> xmlNamespaces = new Dictionary<string, string>();

		public XmlDocWriter(string input, InputType type)
		{
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0049: Expected O, but got Unknown
			if (string.IsNullOrWhiteSpace(input))
			{
				throw new ArgumentException("The input should not be null or empty", "input");
			}
			if (type == InputType.FilePath)
			{
				xmlDoc = LoadXmlFromFileAsync(input).Result;
				return;
			}
			xmlDoc = new XmlDocument();
			xmlDoc.LoadXml(input);
		}

		[SuppressMessage("Microsoft.Design", "CA1054:UriParametersShouldNotBeStrings", Justification = "String makes more sense for XML name space URI")]
		public void AddDefaultNamespace(string namespacePrefix, string namespaceUri)
		{
			RegisterNamespace(namespacePrefix, namespaceUri, isDefault: true);
		}

		[SuppressMessage("Microsoft.Design", "CA1054:UriParametersShouldNotBeStrings", Justification = "String makes more sense for XML name space URI")]
		public void AddNamespace(string prefix, string namespaceUri)
		{
			RegisterNamespace(prefix, namespaceUri, isDefault: false);
		}

		public void SetElementAttribute(string elementPath, string attributeName, string value)
		{
			if (string.IsNullOrWhiteSpace(elementPath))
			{
				throw new ArgumentException("elementPath is null or empty");
			}
			if (string.IsNullOrWhiteSpace(attributeName))
			{
				throw new ArgumentException("attributeName is null or empty");
			}
			XmlElement val = SelectXmlElement(elementPath);
			if (val == null)
			{
				throw new ArgumentException("The XPATH doesn't refer to an existing XML element", "elementPath");
			}
			val.SetAttribute(attributeName, value);
		}

		public void SetElementInnerText(string elementPath, string innerText)
		{
			if (string.IsNullOrWhiteSpace(elementPath))
			{
				throw new ArgumentException("elementPath is null or empty");
			}
			XmlElement val = SelectXmlElement(elementPath);
			if (val == null)
			{
				throw new ArgumentException("The XPATH doesn't refer to an existing XML element", "elementPath");
			}
			val.InnerText = innerText;
		}

		public void AddChildElement(string parentPath, string prefix, string elementName, IReadOnlyDictionary<string, string> attributes, string innerText)
		{
			if (string.IsNullOrWhiteSpace(parentPath))
			{
				throw new ArgumentException("parentPath is null or empty");
			}
			if (string.IsNullOrWhiteSpace(elementName))
			{
				throw new ArgumentException("elementName is null or empty");
			}
			XmlElement val = SelectXmlElement(parentPath);
			if (val == null)
			{
				throw new ArgumentException("The XPATH doesn't refer to an existing XML element", "parentPath");
			}
			XmlElement val2 = null;
			if (string.IsNullOrWhiteSpace(prefix))
			{
				if (string.IsNullOrWhiteSpace(defaultNamespaceUri))
				{
					throw new UtilsException("Default name space hasn't been registered");
				}
				val2 = xmlDoc.CreateElementNS(defaultNamespaceUri, elementName);
			}
			else if (!string.IsNullOrWhiteSpace(defaultNamespacePrefix) && string.Compare(prefix, defaultNamespacePrefix, StringComparison.Ordinal) == 0)
			{
				val2 = xmlDoc.CreateElementNS(defaultNamespaceUri, elementName);
			}
			else
			{
				string value = null;
				if (!xmlNamespaces.TryGetValue(prefix, out value))
				{
					throw new ArgumentException("The name space indicted by prefix isn't found, prefix = " + prefix);
				}
				val2 = xmlDoc.CreateElementNS(value, elementName);
				val2.Prefix = prefix;
			}
			if (attributes != null)
			{
				foreach (KeyValuePair<string, string> attribute in attributes)
				{
					val2.SetAttribute(attribute.Key, attribute.Value);
				}
			}
			if (!string.IsNullOrWhiteSpace(innerText))
			{
				val2.InnerText = innerText;
			}
			val.AppendChild((IXmlNode)(object)val2);
		}

		public bool HasElement(string path)
		{
			if (string.IsNullOrWhiteSpace(path))
			{
				throw new ArgumentException("The element path is null or empty", "path");
			}
			return null != SelectXmlElement(path);
		}

		[SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = " A dictionary won't work here.")]
		public bool QueryQualifyingChildElements(string elementPath, IReadOnlyCollection<KeyValuePair<string, string>> attributes)
		{
			XmlElement val = SelectXmlElement(elementPath);
			if (val == null)
			{
				throw new ArgumentException("The XPATH doesn't refer to an existing XML element", "elementPath");
			}
			if (attributes == null || attributes.Count == 0)
			{
				throw new ArgumentException(" Does not provide at least one qualifying attribute.");
			}
			foreach (IXmlNode item in (IEnumerable<IXmlNode>)val.ChildNodes)
			{
				if (item.Attributes == null)
				{
					continue;
				}
				foreach (KeyValuePair<string, string> attribute in attributes)
				{
					IXmlNode namedItem = item.Attributes.GetNamedItem(attribute.Key);
					if (namedItem != null && namedItem.NodeValue.Equals(attribute.Value))
					{
						LoggerCore.Log("Child Element found with attribute name {0} and value {1}", attribute.Key, attribute.Value);
						return true;
					}
				}
			}
			return false;
		}

		public void RemoveAllChildElements(string elementPath)
		{
			XmlElement val = SelectXmlElement(elementPath);
			if (val == null)
			{
				throw new ArgumentException("The XPATH doesn't refer to an existing XML element", "elementPath");
			}
			List<IXmlNode> list = new List<IXmlNode>();
			foreach (IXmlNode item in (IEnumerable<IXmlNode>)val.ChildNodes)
			{
				list.Add(item);
			}
			foreach (IXmlNode item2 in list)
			{
				val.RemoveChild(item2);
			}
		}

		[SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "The code access the members will be bring back")]
		public void WriteToFile(string filePath)
		{
			if (string.IsNullOrWhiteSpace(filePath))
			{
				throw new ArgumentException("filePath is null or empty");
			}
			SaveXmlToFileAsync(xmlDoc, filePath).Wait();
		}

		public override string ToString()
		{
			return xmlDoc.GetXml();
		}

		private static async Task<XmlDocument> LoadXmlFromFileAsync(string filePath)
		{
			StorageFile file = await StorageFile.GetFileFromPathAsync(filePath);
			return await XmlDocument.LoadFromFileAsync((IStorageFile)(object)file);
		}

		private static async Task SaveXmlToFileAsync(XmlDocument xmlDoc, string filePath)
		{
			string folderPath = Path.GetDirectoryName(filePath);
			if (string.IsNullOrWhiteSpace(folderPath))
			{
				throw new ArgumentException("The file path is invalid", "filePath");
			}
			string fileName = Path.GetFileName(filePath);
			if (string.IsNullOrWhiteSpace(fileName))
			{
				throw new ArgumentException("The file path is invalid", "filePath");
			}
			LoggerCore.Log("Writing XML file. Path: {0}, {1}{2}", filePath, Environment.NewLine, xmlDoc.GetXml());
			await xmlDoc.SaveToFileAsync((IStorageFile)(object)(await (await StorageFolder.GetFolderFromPathAsync(folderPath)).CreateFileAsync(fileName, (CreationCollisionOption)1)));
		}

		private void RegisterNamespace(string namespacePrefix, string namespaceUri, bool isDefault)
		{
			if (string.IsNullOrWhiteSpace(namespacePrefix))
			{
				throw new ArgumentException("prefix is null or empty");
			}
			if (string.IsNullOrWhiteSpace(namespaceUri))
			{
				throw new ArgumentException("namespaceUri is null or empty");
			}
			xmlNamespaces[namespacePrefix] = namespaceUri;
			if (isDefault)
			{
				defaultNamespaceUri = namespaceUri;
				defaultNamespacePrefix = namespacePrefix;
				return;
			}
			string text = "xmlns:" + namespacePrefix;
			if (xmlDoc.DocumentElement.GetAttributeNode(text) == null)
			{
				xmlDoc.DocumentElement.SetAttribute(text, namespaceUri);
			}
		}

		private XmlElement SelectXmlElement(string elementPath)
		{
			string text = BuildNamespaceString();
			IXmlNode val = xmlDoc.SelectSingleNodeNS(elementPath, (object)text);
			if (val != null)
			{
				XmlElement val2 = (XmlElement)(object)((val is XmlElement) ? val : null);
				if (val2 != null)
				{
					return val2;
				}
			}
			return null;
		}

		private string BuildNamespaceString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			bool flag = false;
			foreach (KeyValuePair<string, string> xmlNamespace in xmlNamespaces)
			{
				string key = xmlNamespace.Key;
				string value = xmlNamespace.Value;
				string value2 = string.Format(CultureInfo.InvariantCulture, "xmlns:{0}='{1}'", new object[2] { key, value });
				if (flag)
				{
					stringBuilder.Append(" ");
				}
				stringBuilder.Append(value2);
				flag = true;
			}
			return stringBuilder.ToString();
		}
	}
}