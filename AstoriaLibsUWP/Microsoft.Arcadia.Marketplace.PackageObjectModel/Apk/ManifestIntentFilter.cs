using System;
using System.Collections.Generic;
using System.Xml.Linq;
using Microsoft.Arcadia.Marketplace.Utils.Portable;

namespace Microsoft.Arcadia.Marketplace.PackageObjectModel.Apk
{
	public class ManifestIntentFilter : IDevReportIntentFilter
	{
		private List<string> actions;

		private List<string> categories;

		private List<ManifestIntentFilterData> data;

		public XElement IntentFilterXmlElement { get; private set; }

		public IReadOnlyList<string> Actions => actions;

		public IReadOnlyList<IDevReportIntentFilterData> FilterData => data;

		public IReadOnlyList<string> Categories => categories;

		public IReadOnlyList<ManifestIntentFilterData> Data => data;

		public ManifestIntentFilter(XElement intentFilteryXmlElement)
		{
			if (intentFilteryXmlElement == null)
			{
				throw new ArgumentNullException("intentFilteryXmlElement");
			}
			IntentFilterXmlElement = intentFilteryXmlElement;
			PopulateFields();
		}

		private void PopulateFields()
		{
			actions = new List<string>();
			IEnumerable<XElement> enumerable = IntentFilterXmlElement.Descendants("action");
			foreach (XElement item in enumerable)
			{
				if (XmlUtilites.IsAttributeFound(item, "http://schemas.android.com/apk/res/android", "name"))
				{
					actions.Add(item.Attribute((XNamespace)"http://schemas.android.com/apk/res/android" + "name").Value);
				}
			}
			categories = new List<string>();
			IEnumerable<XElement> enumerable2 = IntentFilterXmlElement.Descendants("category");
			foreach (XElement item2 in enumerable2)
			{
				if (XmlUtilites.IsAttributeFound(item2, "http://schemas.android.com/apk/res/android", "name"))
				{
					categories.Add(item2.Attribute((XNamespace)"http://schemas.android.com/apk/res/android" + "name").Value);
				}
			}
			data = new List<ManifestIntentFilterData>();
			IEnumerable<XElement> enumerable3 = IntentFilterXmlElement.Descendants("data");
			foreach (XElement item3 in enumerable3)
			{
				string text = null;
				string port = null;
				string path = null;
				string text2 = null;
				string text3 = null;
				string pathPrefix = null;
				string pathPattern = null;
				text2 = XmlUtilites.GetAttributeValueForElement(item3, "http://schemas.android.com/apk/res/android", "scheme");
				text3 = XmlUtilites.GetAttributeValueForElement(item3, "http://schemas.android.com/apk/res/android", "mimeType");
				if (text2 != null)
				{
					text = XmlUtilites.GetAttributeValueForElement(item3, "http://schemas.android.com/apk/res/android", "host");
					if (text != null)
					{
						port = XmlUtilites.GetAttributeValueForElement(item3, "http://schemas.android.com/apk/res/android", "port");
						path = XmlUtilites.GetAttributeValueForElement(item3, "http://schemas.android.com/apk/res/android", "path");
						pathPrefix = XmlUtilites.GetAttributeValueForElement(item3, "http://schemas.android.com/apk/res/android", "pathPrefix");
						pathPattern = XmlUtilites.GetAttributeValueForElement(item3, "http://schemas.android.com/apk/res/android", "pathPattern");
					}
				}
				if (text2 != null || text3 != null)
				{
					data.Add(new ManifestIntentFilterData(text, port, path, text2, text3, pathPattern, pathPrefix));
				}
			}
		}
	}
}