using System;
using System.Xml.Linq;
using Microsoft.Arcadia.Marketplace.PackageObjectModel.Portable.Apk;
using Microsoft.Arcadia.Marketplace.Utils.Portable;

namespace Microsoft.Arcadia.Marketplace.PackageObjectModel.Apk
{
	public sealed class ManifestContentProvider
	{
		public XElement ContentProviderXmlElement { get; private set; }

		public ManifestStringResource Name { get; private set; }

		public bool Enabled { get; private set; }

		public ManifestContentProvider(XElement contentProviderXmlElement)
		{
			if (contentProviderXmlElement == null)
			{
				throw new ArgumentNullException("contentProviderXmlElement");
			}
			ContentProviderXmlElement = contentProviderXmlElement;
			PopulateFields();
		}

		private void PopulateFields()
		{
			string attributeValueForElement = XmlUtilites.GetAttributeValueForElement(ContentProviderXmlElement, "http://schemas.android.com/apk/res/android", "name");
			if (!string.IsNullOrEmpty(attributeValueForElement))
			{
				Name = new ManifestStringResource(attributeValueForElement);
			}
			string attributeValueForElement2 = XmlUtilites.GetAttributeValueForElement(ContentProviderXmlElement, "http://schemas.android.com/apk/res/android", "enabled");
			if (string.IsNullOrEmpty(attributeValueForElement2) || string.Compare(attributeValueForElement2.ToUpperInvariant(), "TRUE", StringComparison.Ordinal) == 0)
			{
				Enabled = true;
			}
			else
			{
				Enabled = false;
			}
		}
	}
}