using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Microsoft.Arcadia.Marketplace.Utils.Portable;

namespace Microsoft.Arcadia.Marketplace.PackageObjectModel.Apk
{
	public class ManifestServiceElement : IDevReportManifestService, IDevReportIntentReceiver
	{
		private List<IDevReportIntentFilter> intentFilterList;

		public bool IsExported { get; private set; }

		public string ServiceName { get; private set; }

		public string Permission { get; private set; }

		public IReadOnlyCollection<IDevReportIntentFilter> IntentFilters => intentFilterList;

		public XElement ServiceXmlElement { get; private set; }

		public ManifestServiceElement(XElement serviceXmlElement)
		{
			if (serviceXmlElement == null)
			{
				throw new ArgumentNullException("serviceXmlElement");
			}
			ServiceXmlElement = serviceXmlElement;
			PopulateFields();
		}

		private void PopulateFields()
		{
			ServiceName = XmlUtilites.GetAttributeValueForElement(ServiceXmlElement, "http://schemas.android.com/apk/res/android", "name");
			string attributeValueForElement = XmlUtilites.GetAttributeValueForElement(ServiceXmlElement, "http://schemas.android.com/apk/res/android", "exported");
			IsExported = attributeValueForElement != null && attributeValueForElement.ToUpperInvariant() == bool.TrueString.ToUpperInvariant();
			string text2 = (Permission = (ServiceName = XmlUtilites.GetAttributeValueForElement(ServiceXmlElement, "http://schemas.android.com/apk/res/android", "permission")));
			intentFilterList = new List<IDevReportIntentFilter>(from filterElem in ServiceXmlElement.Elements("intent-filter")
																select new ManifestIntentFilter(filterElem));
		}
	}
}