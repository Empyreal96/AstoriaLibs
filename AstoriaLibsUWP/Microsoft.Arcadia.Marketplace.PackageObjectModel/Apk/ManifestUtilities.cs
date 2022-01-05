using System.Collections.Generic;
using System.Xml.Linq;
using Microsoft.Arcadia.Marketplace.Utils.Portable;

namespace Microsoft.Arcadia.Marketplace.PackageObjectModel.Apk
{
	internal static class ManifestUtilities
	{
		internal static List<DevReportMetadata> GetMetadata(XElement containerElement)
		{
			List<DevReportMetadata> list = new List<DevReportMetadata>();
			IEnumerable<XElement> enumerable = containerElement.Descendants("meta-data");
			foreach (XElement item in enumerable)
			{
				string attributeValueForElement = XmlUtilites.GetAttributeValueForElement(item, "http://schemas.android.com/apk/res/android", "name");
				if (attributeValueForElement != null)
				{
					list.Add(new DevReportMetadata(attributeValueForElement));
				}
			}
			return list;
		}

		internal static List<ManifestIntentFilter> GetIntentFilters(XElement containerElemenet)
		{
			List<ManifestIntentFilter> list = new List<ManifestIntentFilter>();
			IEnumerable<XElement> enumerable = containerElemenet.Descendants("intent-filter");
			foreach (XElement item in enumerable)
			{
				list.Add(new ManifestIntentFilter(item));
			}
			return list;
		}
	}
}