using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Xml.Linq;
using Microsoft.Arcadia.Marketplace.PackageObjectModel.Portable.Apk;
using Microsoft.Arcadia.Marketplace.Utils.Log;
using Microsoft.Arcadia.Marketplace.Utils.Portable;

namespace Microsoft.Arcadia.Marketplace.PackageObjectModel.Apk
{
	public sealed class ManifestApplication : IDevReportManifestApplication
	{
		private const string DefaultLocaleQualifier = "en-US";

		private IList<ManifestActivity> activities;

		private IList<ManifestActivityAlias> activityAliases;

		private IList<ManifestReceiver> receivers;

		private IList<ManifestApplicationMetadata> metadataElements;

		private IList<ManifestContentProvider> contentProviderList;

		public XElement ApplicationXmlElement { get; private set; }

		public ManifestStringResource Label { get; private set; }

		public ManifestStringResource Icon { get; private set; }

		public ManifestApplicationMetadata BackgroundColorData { get; private set; }

		[SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "GMS", Justification = "GMS is an acronym for Google Mobile Services.")]
		public ManifestStringResource GMSVersion { get; private set; }

		public IReadOnlyList<ManifestActivity> Activities => activities.ToList();

		public IReadOnlyList<ManifestActivityAlias> ActivityAliases => activityAliases.ToList();

		public IReadOnlyList<ManifestReceiver> Receivers => receivers.ToList();

		public IReadOnlyList<ManifestApplicationMetadata> MetadataElements => metadataElements.ToList();

		public IReadOnlyCollection<ManifestContentProvider> ContentProviders => contentProviderList.ToList();

		public string Permission { get; private set; }

		public ManifestApplication(XElement applicationXmlElement)
		{
			if (applicationXmlElement == null)
			{
				throw new ArgumentNullException("applicationXmlElement");
			}
			ApplicationXmlElement = applicationXmlElement;
			PopulateFields();
		}

		private void PopulateFields()
		{
			PopulateLabel();
			PopulateIcon();
			PopulateActivities();
			PopulateActivityAliases();
			PopulateReceivers();
			PopulateMetadata();
			PopulateContentProvider();
			PopulateGMSVersion();
			PopulatePermission();
		}

		private void PopulateGMSVersion()
		{
			XElement xElement = (from el in ApplicationXmlElement.Descendants("meta-data")
								 where el.Attribute((XNamespace)"http://schemas.android.com/apk/res/android" + "name").Value == "com.google.android.gms.version"
								 select el).FirstOrDefault();
			if (xElement != null)
			{
				string attributeValueForElement = XmlUtilites.GetAttributeValueForElement(xElement, "http://schemas.android.com/apk/res/android", "value");
				if (!string.IsNullOrEmpty(attributeValueForElement))
				{
					GMSVersion = new ManifestStringResource(attributeValueForElement);
				}
			}
		}

		private void PopulateLabel()
		{
			string attributeValueForElement = XmlUtilites.GetAttributeValueForElement(ApplicationXmlElement, "http://schemas.android.com/apk/res/android", "label");
			if (!string.IsNullOrEmpty(attributeValueForElement))
			{
				Label = new ManifestStringResource(attributeValueForElement);
			}
		}

		public string RetrieveEnglishLabelAsString(IDictionary<uint, ApkResource> resources)
		{
			if (Label != null)
			{
				if (Label.IsResource)
				{
					ApkResource resource = ApkResourceHelper.GetResource(Label, resources);
					string text = (from oneValue in resource.Values
								   where !string.IsNullOrWhiteSpace(oneValue.Config.Locale) && oneValue.Config.Locale.ToUpper().Equals("en-US".ToUpper())
								   select oneValue.Value).FirstOrDefault();
					if (string.IsNullOrWhiteSpace(text))
					{
						LoggerCore.Log("A value with config explicitly set to english not found.");
						text = (from oneValue in resource.Values
								where string.IsNullOrWhiteSpace(oneValue.Config.Locale)
								select oneValue.Value).FirstOrDefault();
					}
					if (string.IsNullOrWhiteSpace(text))
					{
						LoggerCore.Log("English label from manifest is not found. Just returning the first value");
						text = resource.Values[0].Value;
					}
					LoggerCore.Log("English label calculated as: {0}", text);
					return text;
				}
				return Label.Content;
			}
			return null;
		}

		public IEnumerable<ManifestReceiver> FindReceiversWithAction(string actionName)
		{
			foreach (ManifestReceiver receiver in Receivers)
			{
				foreach (ManifestIntentFilter filter in receiver.Filters)
				{
					if (filter.Actions.Contains(actionName))
					{
						yield return receiver;
					}
				}
			}
		}

		private void PopulateIcon()
		{
			string attributeValueForElement = XmlUtilites.GetAttributeValueForElement(ApplicationXmlElement, "http://schemas.android.com/apk/res/android", "icon");
			if (!string.IsNullOrEmpty(attributeValueForElement))
			{
				Icon = new ManifestStringResource(attributeValueForElement);
			}
		}

		private void PopulateActivities()
		{
			activities = new List<ManifestActivity>();
			IEnumerable<XElement> enumerable = ApplicationXmlElement.Descendants("activity");
			foreach (XElement item in enumerable)
			{
				activities.Add(new ManifestActivity(item));
			}
		}

		private void PopulateActivityAliases()
		{
			activityAliases = new List<ManifestActivityAlias>();
			IEnumerable<XElement> enumerable = ApplicationXmlElement.Descendants("activity-alias");
			foreach (XElement item in enumerable)
			{
				activityAliases.Add(new ManifestActivityAlias(item));
			}
		}

		private void PopulateReceivers()
		{
			receivers = new List<ManifestReceiver>();
			IEnumerable<XElement> enumerable = ApplicationXmlElement.Descendants("receiver");
			foreach (XElement item in enumerable)
			{
				receivers.Add(new ManifestReceiver(item));
			}
		}

		private void PopulateMetadata()
		{
			metadataElements = new List<ManifestApplicationMetadata>();
			IEnumerable<XElement> enumerable = ApplicationXmlElement.Descendants("meta-data");
			foreach (XElement item in enumerable)
			{
				ManifestApplicationMetadata manifestApplicationMetadata = new ManifestApplicationMetadata(item);
				metadataElements.Add(manifestApplicationMetadata);
				if (string.Compare(manifestApplicationMetadata.Name, "windows-background", StringComparison.OrdinalIgnoreCase) == 0)
				{
					BackgroundColorData = manifestApplicationMetadata;
				}
			}
		}

		private void PopulateContentProvider()
		{
			contentProviderList = new List<ManifestContentProvider>();
			IEnumerable<XElement> enumerable = ApplicationXmlElement.Descendants("provider");
			foreach (XElement item in enumerable)
			{
				contentProviderList.Add(new ManifestContentProvider(item));
			}
		}

		private void PopulatePermission()
		{
			Permission = XmlUtilites.GetAttributeValueForElement(ApplicationXmlElement, "http://schemas.android.com/apk/res/android", "permission");
		}
	}
}