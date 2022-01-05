using System;
using System.Xml.Linq;
using Microsoft.Arcadia.Marketplace.Utils.Log;
using Microsoft.Arcadia.Marketplace.Utils.Portable;

namespace Microsoft.Arcadia.Marketplace.PackageObjectModel.Portable.Apk
{
	public sealed class ApkConfigFile
	{
		public GameServicesConfig GameServices { get; private set; }

		public AnalyticsConfig Analytics { get; private set; }

		private XDocument XmlDoc { get; set; }

		public ApkConfigFile(XDocument configXml)
		{
			if (configXml == null)
			{
				throw new ArgumentNullException("configXml");
			}
			XmlDoc = configXml;
			PopulateFields();
		}

		private void PopulateFields()
		{
			LoggerCore.Log("APK Config File, populating fields");
			foreach (XElement item in XmlDoc.Descendants("service"))
			{
				string attributeValueForElement = XmlUtilites.GetAttributeValueForElement(item, string.Empty, "name");
				if (string.Compare(attributeValueForElement, "games", StringComparison.OrdinalIgnoreCase) == 0)
				{
					PopulateGamesConfig(item);
				}
				else if (string.Compare(attributeValueForElement, "analytics", StringComparison.OrdinalIgnoreCase) == 0)
				{
					PopulateAnalytics(item);
				}
			}
		}

		private void PopulateGamesConfig(XElement serviceElement)
		{
			GameServices = new GameServicesConfig();
			XElement xElement = serviceElement.Element("titleId");
			if (xElement != null)
			{
				GameServices.TitleId = xElement.Value;
			}
			XElement xElement2 = serviceElement.Element("primaryServiceConfigId");
			if (xElement2 != null)
			{
				GameServices.PrimaryServiceConfigId = xElement2.Value;
			}
			XElement xElement3 = serviceElement.Element("sandbox");
			if (xElement3 != null)
			{
				GameServices.Sandbox = xElement3.Value;
			}
			XElement xElement4 = serviceElement.Element("useDeviceToken");
			if (xElement4 != null)
			{
				GameServices.UseDeviceToken = xElement4.Value;
			}
		}

		private void PopulateAnalytics(XElement serviceElement)
		{
			Analytics = new AnalyticsConfig();
			XElement xElement = serviceElement.Element("key");
			if (xElement != null)
			{
				Analytics.AnalyticsKey = xElement.Value;
			}
		}
	}
}