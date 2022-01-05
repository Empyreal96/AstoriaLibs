using System;
using System.Collections.Generic;
using System.Xml.Linq;
using Microsoft.Arcadia.Marketplace.PackageObjectModel.Portable.Apk;
using Microsoft.Arcadia.Marketplace.Utils.Portable;

namespace Microsoft.Arcadia.Marketplace.PackageObjectModel.Apk
{
	public sealed class ManifestReceiver : IDevReportReceiver, IDevReportIntentReceiver
	{
		private List<ManifestIntentFilter> filters;

		public XElement ReceiverXmlElement { get; private set; }

		public ManifestStringResource NameResource { get; private set; }

		public ManifestString PermissionString { get; private set; }

		public bool IsEnabled { get; private set; }

		public IReadOnlyList<ManifestIntentFilter> Filters => filters;

		public string Permission
		{
			get
			{
				if (PermissionString != null)
				{
					return PermissionString.Content;
				}
				return null;
			}
		}

		public IReadOnlyCollection<IDevReportIntentFilter> IntentFilters
		{
			get
			{
				if (filters != null)
				{
					return filters;
				}
				return new List<IDevReportIntentFilter>();
			}
		}

		public ManifestReceiver(XElement receiverXmlElement)
		{
			if (receiverXmlElement == null)
			{
				throw new ArgumentNullException("receiverXmlElement");
			}
			ReceiverXmlElement = receiverXmlElement;
			PopulateFields();
		}

		private void PopulateFields()
		{
			string attributeValueForElement = XmlUtilites.GetAttributeValueForElement(ReceiverXmlElement, "http://schemas.android.com/apk/res/android", "name");
			if (!string.IsNullOrEmpty(attributeValueForElement))
			{
				NameResource = new ManifestStringResource(attributeValueForElement);
			}
			string attributeValueForElement2 = XmlUtilites.GetAttributeValueForElement(ReceiverXmlElement, "http://schemas.android.com/apk/res/android", "permission");
			if (!string.IsNullOrEmpty(attributeValueForElement2))
			{
				PermissionString = new ManifestString("permission", attributeValueForElement2);
			}
			string attributeValueForElement3 = XmlUtilites.GetAttributeValueForElement(ReceiverXmlElement, "http://schemas.android.com/apk/res/android", "enabled");
			if (!string.IsNullOrEmpty(attributeValueForElement3))
			{
				if (bool.TryParse(attributeValueForElement3, out var result))
				{
					IsEnabled = result;
				}
			}
			else
			{
				IsEnabled = true;
			}
			filters = new List<ManifestIntentFilter>();
			IEnumerable<XElement> enumerable = ReceiverXmlElement.Descendants("intent-filter");
			foreach (XElement item in enumerable)
			{
				filters.Add(new ManifestIntentFilter(item));
			}
		}
	}
}