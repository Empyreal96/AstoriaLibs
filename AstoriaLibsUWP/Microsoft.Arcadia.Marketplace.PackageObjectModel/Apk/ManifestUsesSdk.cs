using System;
using System.Xml.Linq;
using Microsoft.Arcadia.Marketplace.PackageObjectModel.Portable.Apk;
using Microsoft.Arcadia.Marketplace.Utils.Portable;

namespace Microsoft.Arcadia.Marketplace.PackageObjectModel.Apk
{
	public sealed class ManifestUsesSdk
	{
		public ManifestStringResource MinSdkVersion { get; private set; }

		public ManifestStringResource MaxSdkVersion { get; private set; }

		public ManifestStringResource TargetSdkVersion { get; private set; }

		private XElement UsesSdkXmlElement { get; set; }

		public ManifestUsesSdk(XElement usesSdkElement)
		{
			if (usesSdkElement == null)
			{
				throw new ArgumentNullException("usesSdkElement");
			}
			UsesSdkXmlElement = usesSdkElement;
			PopulateFields();
		}

		private static ManifestStringResource PopulateSdkVersion(XElement element, string manifestusessdkversionattribute)
		{
			ManifestStringResource result = null;
			if (XmlUtilites.IsAttributeFound(element, "http://schemas.android.com/apk/res/android", manifestusessdkversionattribute))
			{
				result = new ManifestStringResource(XmlUtilites.GetAttributeValueForElement(element, "http://schemas.android.com/apk/res/android", manifestusessdkversionattribute));
			}
			return result;
		}

		private void PopulateFields()
		{
			MaxSdkVersion = PopulateSdkVersion(UsesSdkXmlElement, "maxSdkVersion");
			MinSdkVersion = PopulateSdkVersion(UsesSdkXmlElement, "minSdkVersion");
			TargetSdkVersion = PopulateSdkVersion(UsesSdkXmlElement, "targetSdkVersion");
		}
	}
}