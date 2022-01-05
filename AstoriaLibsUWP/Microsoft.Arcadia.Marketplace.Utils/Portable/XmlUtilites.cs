using System;
using System.Linq;
using System.Xml.Linq;
using Microsoft.Arcadia.Marketplace.Utils.Log;

namespace Microsoft.Arcadia.Marketplace.Utils.Portable
{
	public static class XmlUtilites
	{
		public static string MakeElementPath(string prefix, params string[] elementNames)
		{
			if (elementNames == null)
			{
				throw new ArgumentNullException("elementNames");
			}
			if (elementNames.Length == 0)
			{
				throw new ArgumentException("elementNames is an empty array");
			}
			string text = string.Empty;
			for (int i = 0; i < elementNames.Length; i++)
			{
				if (i > 0)
				{
					text += "/";
				}
				text += (string.IsNullOrEmpty(prefix) ? elementNames[i] : (prefix + ":" + elementNames[i]));
			}
			LoggerCore.Log("Make Element Path, path = " + text);
			return text;
		}

		public static bool IsAttributeEqual(XElement elemt, XNamespace ns, string attribute, string value, bool caseSensitive)
		{
			if (elemt != null)
			{
				XAttribute xAttribute = elemt.Attribute(ns + attribute);
				if (xAttribute != null)
				{
					StringComparison stringComparison = StringComparison.CurrentCulture;
					stringComparison = ((!caseSensitive) ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal);
					if (string.Compare(xAttribute.Value, value, stringComparison) == 0)
					{
						return true;
					}
				}
			}
			return false;
		}

		public static bool IsAttributeFound(XElement elemt, XNamespace ns, string attribute)
		{
			if (elemt != null)
			{
				XAttribute xAttribute = elemt.Attribute(ns + attribute);
				if (xAttribute != null)
				{
					return true;
				}
			}
			return false;
		}

		public static string GetAttributeValueForElement(XElement element, XNamespace attributeNamespace, string attributeName)
		{
			if (element == null)
			{
				throw new ArgumentNullException("element");
			}
			XAttribute xAttribute = (from attribute in element.Attributes(attributeNamespace + attributeName)
									 select (attribute)).FirstOrDefault();
			if (xAttribute != null)
			{
				LoggerCore.Log("Found attribute: attribute namespace = {0}, name = {1}, value = {2} for element: name = {3}", attributeNamespace, attributeName, xAttribute.Value, element.Value);
				return xAttribute.Value;
			}
			LoggerCore.Log("Can't find attribute with attribute namespace: {0} and name: {1}, under element: {2}", attributeNamespace, attributeName, element.Value);
			return null;
		}
	}
}