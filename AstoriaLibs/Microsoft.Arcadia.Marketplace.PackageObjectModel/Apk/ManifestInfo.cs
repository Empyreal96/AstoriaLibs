using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Microsoft.Arcadia.Marketplace.PackageObjectModel.Portable;
using Microsoft.Arcadia.Marketplace.PackageObjectModel.Portable.Apk;
using Microsoft.Arcadia.Marketplace.PackageObjectModel.Portable.Appx;
using Microsoft.Arcadia.Marketplace.Utils.Log;
using Microsoft.Arcadia.Marketplace.Utils.Portable;

namespace Microsoft.Arcadia.Marketplace.PackageObjectModel.Apk
{
	public sealed class ManifestInfo : IDevReportManifestObjectModel
	{
		private IList<ManifestUsesFeature> usesFeatureList;

		private IList<ManifestUsesPermission> usesPermissionList;

		private IList<ManifestServiceElement> servicesList;

		private HashSet<string> allPermissions;

		private IReadOnlyCollection<IDevReportActivity> contradictingActivites;

		private HashSet<string> allUsesFeatures;

		public int MinSdkVersion { get; private set; }

		public int TargetSdkVersion { get; private set; }

		public int? MaxSdkVersion { get; private set; }

		public WindowsOSVersion MinWindowsOSVersion { get; set; }

		public string PackageName
		{
			get
			{
				if (PackageNameResource != null)
				{
					return PackageNameResource.Content;
				}
				return null;
			}
		}

		public string VersionCodeValue { get; private set; }

		public IDevReportManifestApplication ManifestApplication => Application;

		public IReadOnlyCollection<string> AllPermissions => allPermissions.ToList();

		public IReadOnlyCollection<string> AllUsesFeatures => allUsesFeatures.ToList();

		public IReadOnlyCollection<IDevReportActivity> AllActivities
		{
			get
			{
				if (Application != null && Application.Activities != null)
				{
					return Application.Activities;
				}
				return new List<IDevReportActivity>();
			}
		}

		public IReadOnlyCollection<IDevReportActivityAlias> AllActivityAliases
		{
			get
			{
				if (Application != null && Application.ActivityAliases != null)
				{
					return Application.ActivityAliases;
				}
				return new List<IDevReportActivityAlias>();
			}
		}

		public IReadOnlyCollection<IDevReportReceiver> AllReceivers
		{
			get
			{
				if (Application != null && Application.Receivers != null)
				{
					return Application.Receivers;
				}
				return new List<IDevReportReceiver>();
			}
		}

		public IReadOnlyCollection<IDevReportManifestService> AllServices
		{
			get
			{
				if (servicesList == null)
				{
					return new List<IDevReportManifestService>();
				}
				return servicesList.Where((IDevReportManifestService m) => !string.IsNullOrWhiteSpace(m.ServiceName)).ToList();
			}
		}

		public string ActualPackageName { get; private set; }

		public string ActualVersionName { get; private set; }

		public ManifestApplication Application { get; private set; }

		public IReadOnlyCollection<ManifestServiceElement> Services => servicesList.ToList();

		public ManifestStringResource PackageNameResource { get; private set; }

		public IReadOnlyCollection<ManifestUsesFeature> UsesFeatures => usesFeatureList.ToList();

		public IReadOnlyCollection<ManifestUsesPermission> UsesPermissions => usesPermissionList.ToList();

		public ManifestUsesSdk UsesSdk { get; private set; }

		public ManifestStringResource VersionCode { get; private set; }

		public ManifestStringResource VersionName { get; private set; }

		public XDocument XmlDoc { get; private set; }

		public IReadOnlyCollection<IDevReportActivity> ContradictingActivities
		{
			get
			{
				if (contradictingActivites == null)
				{
					if (Application != null && Application.Activities != null)
					{
						contradictingActivites = RetrieveContradicingRotatingActivities();
					}
					else
					{
						contradictingActivites = new List<IDevReportActivity>();
					}
				}
				return contradictingActivites;
			}
		}

		public ManifestInfo(XDocument manifestXml, IDictionary<uint, ApkResource> resources)
		{
			if (manifestXml == null)
			{
				throw new ArgumentNullException("manifestXml");
			}
			XmlDoc = manifestXml;
			PopulateFields(resources);
		}

		public ManifestInfo(XDocument manifestXml)
			: this(manifestXml, null)
		{
		}

		public bool HasActivity(string activityName, bool partialMatch)
		{
			if (partialMatch)
			{
				return Application.Activities.Any((ManifestActivity a) => a.Name.Content.Contains(activityName));
			}
			return Application.Activities.Any((ManifestActivity a) => a.Name.Content.Equals(activityName));
		}

		public bool HasLibrary(string libraryName)
		{
			bool result = false;
			foreach (XElement item in Application.ApplicationXmlElement.Descendants("uses-library"))
			{
				if (XmlUtilites.IsAttributeEqual(item, "http://schemas.android.com/apk/res/android", "name", libraryName, caseSensitive: true))
				{
					return true;
				}
			}
			return result;
		}

		public bool HasMetadata(string metadataName, bool partialMatch)
		{
			if (partialMatch)
			{
				return Application.MetadataElements.Any((ManifestApplicationMetadata m) => m.Name.Contains(metadataName));
			}
			return Application.MetadataElements.Any((ManifestApplicationMetadata m) => m.Name.Equals(metadataName));
		}

		public bool HasPermission(string permissionName)
		{
			return UsesPermissions.Any((ManifestUsesPermission p) => p.Name.Content.Equals(permissionName));
		}

		private IReadOnlyCollection<IDevReportActivity> RetrieveContradicingRotatingActivities()
		{
			ManifestActivity manifestActivity = Application.Activities.Where((ManifestActivity a) => a.HasMainActivity).FirstOrDefault();
			if (manifestActivity == null)
			{
				return new List<IDevReportActivity>();
			}
			ApkScreenOrientationType screenOrientation = manifestActivity.ScreenOrientation;
			IList<ManifestActivity> supportingActivityList = Application.Activities.Where((ManifestActivity a) => !a.HasMainActivity && a.ScreenOrientation != ApkScreenOrientationType.Undeclared).ToList();
			return ScreenOrientationMap.GetContradictionRotatingActivites(screenOrientation, supportingActivityList);
		}

		private void PopulateFields(IDictionary<uint, ApkResource> resources)
		{
			MinWindowsOSVersion = WindowsOSVersion.ThresholdTH1;
			string attributeValueForElement = XmlUtilites.GetAttributeValueForElement(XmlDoc.Root, string.Empty, "package");
			PopulatePackageName(resources, attributeValueForElement);
			string attributeValueForElement2 = XmlUtilites.GetAttributeValueForElement(XmlDoc.Root, "http://schemas.android.com/apk/res/android", "versionCode");
			PopulateVersionCode(resources, attributeValueForElement2);
			string attributeValueForElement3 = XmlUtilites.GetAttributeValueForElement(XmlDoc.Root, "http://schemas.android.com/apk/res/android", "versionName");
			PopulateVersionName(resources, attributeValueForElement3);
			LoggerCore.Log("Manifest Info - Package Name: {0}, Version Code: {1}, Version Name: {2}", attributeValueForElement, attributeValueForElement2, attributeValueForElement3);
			XElement xElement = XmlDoc.Descendants("application").FirstOrDefault();
			if (xElement == null)
			{
				throw new DecoderManifestNoApplicationException("No Application element is found.");
			}
			Application = new ManifestApplication(xElement);
			servicesList = new List<ManifestServiceElement>();
			foreach (XElement item4 in XmlDoc.Descendants("service"))
			{
				ManifestServiceElement item = new ManifestServiceElement(item4);
				servicesList.Add(item);
			}
			MinSdkVersion = 1;
			TargetSdkVersion = 1;
			PopulateUsesSdk();
			usesPermissionList = new List<ManifestUsesPermission>();
			foreach (XElement item5 in XmlDoc.Descendants("uses-permission"))
			{
				ManifestUsesPermission item2 = new ManifestUsesPermission(item5, TargetSdkVersion);
				usesPermissionList.Add(item2);
			}
			allPermissions = new HashSet<string>();
			if (MinSdkVersion < 4 && TargetSdkVersion < 4)
			{
				allPermissions.Add("android.permission.READ_PHONE_STATE");
				allPermissions.Add("android.permission.WRITE_EXTERNAL_STORAGE");
				allPermissions.Add("android.permission.READ_EXTERNAL_STORAGE");
			}
			foreach (ManifestUsesPermission usesPermission in usesPermissionList)
			{
				string content = usesPermission.Name.Content;
				allPermissions.Add(content);
				foreach (string impliedPermission in usesPermission.ImpliedPermissions)
				{
					allPermissions.Add(impliedPermission);
				}
			}
			usesFeatureList = new List<ManifestUsesFeature>();
			foreach (XElement item6 in XmlDoc.Descendants("uses-feature"))
			{
				ManifestUsesFeature item3 = new ManifestUsesFeature(item6);
				usesFeatureList.Add(item3);
			}
			allUsesFeatures = new HashSet<string>();
			foreach (ManifestUsesFeature usesFeature in usesFeatureList)
			{
				if (usesFeature.Name != null)
				{
					allUsesFeatures.Add(usesFeature.Name.Content);
				}
			}
		}

		private void PopulateUsesSdk()
		{
			XElement xElement = XmlDoc.Descendants("uses-sdk").FirstOrDefault();
			if (xElement == null)
			{
				return;
			}
			UsesSdk = new ManifestUsesSdk(xElement);
			if (UsesSdk != null)
			{
				if (UsesSdk.MinSdkVersion != null && int.TryParse(UsesSdk.MinSdkVersion.Content, out var result))
				{
					MinSdkVersion = result;
					TargetSdkVersion = result;
					LoggerCore.Log("Successfully parsed the minSdkVersion {0}", MinSdkVersion);
				}
				if (UsesSdk.TargetSdkVersion != null && int.TryParse(UsesSdk.TargetSdkVersion.Content, out result))
				{
					TargetSdkVersion = result;
					LoggerCore.Log("Successfully parsed the targetSdkVersion {0}", TargetSdkVersion);
				}
				if (UsesSdk.MaxSdkVersion != null && int.TryParse(UsesSdk.MaxSdkVersion.Content, out result))
				{
					MaxSdkVersion = result;
					LoggerCore.Log("Successfully parsed the maxSdkVersion {0}", MaxSdkVersion);
				}
			}
		}

		private void PopulateVersionName(IDictionary<uint, ApkResource> resources, string versionName)
		{
			if (string.IsNullOrWhiteSpace(versionName))
			{
				return;
			}
			VersionName = new ManifestStringResource(versionName);
			if (VersionName.IsResource && resources != null)
			{
				ApkResource resource = ApkResourceHelper.GetResource(VersionName, resources);
				if (resource.Values.Any())
				{
					ActualVersionName = resource.Values.First().Value;
				}
			}
		}

		private void PopulateVersionCode(IDictionary<uint, ApkResource> resources, string versionCode)
		{
			if (!string.IsNullOrWhiteSpace(versionCode))
			{
				VersionCode = new ManifestStringResource(versionCode);
				if (!VersionCode.IsResource)
				{
					uint result = 0u;
					if (!uint.TryParse(VersionCode.Content, out result))
					{
						throw new DecoderManifestNoVersionCodeException("Invalid version code provided.");
					}
					VersionCodeValue = VersionCode.Content;
				}
				else if (resources != null)
				{
					ApkResource resource = ApkResourceHelper.GetResource(VersionCode, resources);
					if (resource.Values.Any())
					{
						VersionCodeValue = resource.Values.First().Value;
					}
				}
				return;
			}
			throw new DecoderManifestNoVersionCodeException("No version code is found.");
		}

		private void PopulatePackageName(IDictionary<uint, ApkResource> resources, string packageName)
		{
			if (string.IsNullOrWhiteSpace(packageName))
			{
				return;
			}
			PackageNameResource = new ManifestStringResource(packageName);
			if (PackageNameResource.IsResource && resources != null)
			{
				ApkResource resource = ApkResourceHelper.GetResource(PackageNameResource, resources);
				if (resource.Values.Any())
				{
					ActualPackageName = resource.Values.First().Value;
				}
			}
		}
	}
}