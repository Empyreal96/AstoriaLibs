using System;
using System.Diagnostics.CodeAnalysis;
using System.Xml.Linq;
using Microsoft.Arcadia.Marketplace.PackageObjectModel.Portable.Apk;
using Microsoft.Arcadia.Marketplace.Utils.Portable;

namespace Microsoft.Arcadia.Marketplace.PackageObjectModel.Apk
{
	[SuppressMessage("Microsoft.Naming", "CA1711:IdentifiersShouldNotHaveIncorrectSuffix", Justification = "Explicitly want to name this class ending with feature.")]
	public sealed class ManifestUsesFeature
	{
		public XElement UsesFeatureXmlElement { get; private set; }

		public ManifestStringResource Name { get; private set; }

		public bool Required { get; private set; }

		public ManifestUsesFeature(XElement usesFeatureXmlElement)
		{
			if (usesFeatureXmlElement == null)
			{
				throw new ArgumentNullException("usesFeatureXmlElement");
			}
			UsesFeatureXmlElement = usesFeatureXmlElement;
			PopulateFields();
		}

		private void PopulateFields()
		{
			string attributeValueForElement = XmlUtilites.GetAttributeValueForElement(UsesFeatureXmlElement, "http://schemas.android.com/apk/res/android", "name");
			if (!string.IsNullOrEmpty(attributeValueForElement))
			{
				Name = new ManifestStringResource(attributeValueForElement);
			}
			string attributeValueForElement2 = XmlUtilites.GetAttributeValueForElement(UsesFeatureXmlElement, "http://schemas.android.com/apk/res/android", "required");
			if (!string.IsNullOrEmpty(attributeValueForElement2))
			{
				if (bool.TryParse(attributeValueForElement2, out var result))
				{
					Required = result;
				}
			}
			else
			{
				Required = true;
			}
		}
	}
}