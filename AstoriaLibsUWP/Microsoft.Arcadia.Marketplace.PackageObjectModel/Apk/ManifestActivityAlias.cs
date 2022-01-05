using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Microsoft.Arcadia.Marketplace.PackageObjectModel.Portable.Apk;
using Microsoft.Arcadia.Marketplace.Utils.Portable;

namespace Microsoft.Arcadia.Marketplace.PackageObjectModel.Apk
{
	public sealed class ManifestActivityAlias : IDevReportActivityAlias, IDevReportIntentReceiver, IDevReportMetadataContainer
	{
		private List<ManifestIntentFilter> filters;

		private List<DevReportMetadata> metadata;

		public XElement ActivityAliasXmlElement { get; private set; }

		public ManifestStringResource Name { get; private set; }

		public ManifestStringResource Label { get; private set; }

		public ManifestStringResource TargetActivity { get; private set; }

		public IReadOnlyList<ManifestIntentFilter> Filters => filters;

		public bool HasMainActivity { get; private set; }

		public bool IsLauncherCategory { get; private set; }

		public bool IsHomeCategory { get; private set; }

		public string TargetActivityString
		{
			get
			{
				if (TargetActivity != null)
				{
					return TargetActivity.Content;
				}
				return null;
			}
		}

		public IReadOnlyCollection<IDevReportIntentFilter> IntentFilters => filters;

		public IReadOnlyCollection<DevReportMetadata> Metadata => metadata;

		public ManifestActivityAlias(XElement activityAliasXmlElement)
		{
			if (activityAliasXmlElement == null)
			{
				throw new ArgumentNullException("activityAliasXmlElement");
			}
			ActivityAliasXmlElement = activityAliasXmlElement;
			PopulateFields();
		}

		private void PopulateFields()
		{
			string attributeValueForElement = XmlUtilites.GetAttributeValueForElement(ActivityAliasXmlElement, "http://schemas.android.com/apk/res/android", "name");
			if (!string.IsNullOrEmpty(attributeValueForElement))
			{
				Name = new ManifestStringResource(attributeValueForElement);
			}
			string attributeValueForElement2 = XmlUtilites.GetAttributeValueForElement(ActivityAliasXmlElement, "http://schemas.android.com/apk/res/android", "label");
			if (!string.IsNullOrEmpty(attributeValueForElement2))
			{
				Label = new ManifestStringResource(attributeValueForElement2);
			}
			string attributeValueForElement3 = XmlUtilites.GetAttributeValueForElement(ActivityAliasXmlElement, "http://schemas.android.com/apk/res/android", "targetActivity");
			if (!string.IsNullOrEmpty(attributeValueForElement3))
			{
				TargetActivity = new ManifestStringResource(attributeValueForElement3);
			}
			filters = ManifestUtilities.GetIntentFilters(ActivityAliasXmlElement);
			foreach (ManifestIntentFilter filter in filters)
			{
				if (filter.Actions.Contains("android.intent.action.MAIN"))
				{
					HasMainActivity = true;
				}
				if (filter.Categories.Contains("android.intent.category.LAUNCHER"))
				{
					IsLauncherCategory = true;
				}
				if (filter.Categories.Contains("android.intent.category.HOME"))
				{
					IsHomeCategory = true;
				}
				if (HasMainActivity && IsLauncherCategory && IsHomeCategory)
				{
					break;
				}
			}
			metadata = ManifestUtilities.GetMetadata(ActivityAliasXmlElement);
		}
	}
}