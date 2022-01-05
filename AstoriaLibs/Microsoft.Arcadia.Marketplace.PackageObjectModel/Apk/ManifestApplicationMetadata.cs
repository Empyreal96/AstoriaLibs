using System;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using Microsoft.Arcadia.Marketplace.PackageObjectModel.Portable.Apk;
using Microsoft.Arcadia.Marketplace.Utils.Log;
using Microsoft.Arcadia.Marketplace.Utils.Portable;

namespace Microsoft.Arcadia.Marketplace.PackageObjectModel.Apk
{
	public sealed class ManifestApplicationMetadata
	{
		public XElement MetadataXmlElement { get; private set; }

		public AppxPackageType PackageType { get; private set; }

		public AppxImageType ImageType { get; private set; }

		public string ScaleQualifier { get; private set; }

		public ManifestStringResource Resource { get; private set; }

		public ManifestStringResource Value { get; private set; }

		public string Name { get; private set; }

		public bool IsValidAppxResource { get; private set; }

		public ManifestApplicationMetadata(XElement metadataXElement)
		{
			MetadataXmlElement = metadataXElement;
			PopulateFields();
		}

		private void PopulateFields()
		{
			IsValidAppxResource = false;
			string attributeValueForElement = XmlUtilites.GetAttributeValueForElement(MetadataXmlElement, "http://schemas.android.com/apk/res/android", "name");
			if (!string.IsNullOrEmpty(attributeValueForElement))
			{
				Name = attributeValueForElement.Trim();
				string attributeValueForElement2 = XmlUtilites.GetAttributeValueForElement(MetadataXmlElement, "http://schemas.android.com/apk/res/android", "resource");
				if (!string.IsNullOrEmpty(attributeValueForElement2))
				{
					TryPopulateAsAppxResourceMetadata(Name, attributeValueForElement2);
				}
			}
			attributeValueForElement = XmlUtilites.GetAttributeValueForElement(MetadataXmlElement, "http://schemas.android.com/apk/res/android", "value");
			if (!string.IsNullOrEmpty(attributeValueForElement))
			{
				ManifestStringResource manifestStringResource2 = (Value = new ManifestStringResource(attributeValueForElement.Trim()));
			}
		}

		private void TryPopulateAsAppxResourceMetadata(string name, string resource)
		{
			ManifestStringResource manifestStringResource = new ManifestStringResource(resource);
			if (manifestStringResource.IsResource)
			{
				if (Regex.IsMatch(name, "Windows\\.(applogo|storelogo|tilelogosmall|tilelogomedium|tilelogolarge|tilelogowide|splashscreen)\\.scale-(80|100|140|180)", RegexOptions.IgnoreCase))
				{
					char[] separator = new char[1] { '.' };
					string[] array = name.Split(separator);
					PopulateAppxResourceMetadata(AppxPackageType.Tablet, array[1], array[2]);
				}
				else if (Regex.IsMatch(name, "Windows\\.phone\\.(applogo|storelogo|tilelogosmall|tilelogomedium|tilelogowide|splashscreen)\\.scale-(100|140|240)", RegexOptions.IgnoreCase))
				{
					char[] separator2 = new char[1] { '.' };
					string[] array2 = name.Split(separator2);
					PopulateAppxResourceMetadata(AppxPackageType.Phone, array2[2], array2[3]);
				}
			}
			Resource = manifestStringResource;
		}

		private void PopulateAppxResourceMetadata(AppxPackageType packageType, string imageTypeAsString, string scaleQualifier)
		{
			LoggerCore.Log("Type of Image = {0}, Scale-Qualifier = {1}", imageTypeAsString, scaleQualifier);
			if (Enum.TryParse<AppxImageType>(imageTypeAsString, ignoreCase: true, out var result))
			{
				PackageType = packageType;
				ImageType = result;
				ScaleQualifier = scaleQualifier;
				IsValidAppxResource = true;
			}
		}
	}
}