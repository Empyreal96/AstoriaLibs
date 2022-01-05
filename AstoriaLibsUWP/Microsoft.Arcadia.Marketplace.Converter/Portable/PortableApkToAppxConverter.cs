using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.Arcadia.Marketplace.IconProcessor.Imaging;
using Microsoft.Arcadia.Marketplace.PackageObjectModel;
using Microsoft.Arcadia.Marketplace.PackageObjectModel.Apk;
using Microsoft.Arcadia.Marketplace.PackageObjectModel.Permission;
using Microsoft.Arcadia.Marketplace.PackageObjectModel.Portable.Apk;
using Microsoft.Arcadia.Marketplace.PackageObjectModel.Portable.Appx;
using Microsoft.Arcadia.Marketplace.Utils.Log;
using Microsoft.Arcadia.Marketplace.Utils.Portable;

namespace Microsoft.Arcadia.Marketplace.Converter.Portable
{
	public class PortableApkToAppxConverter
	{
		protected const string StoreManifestFileName = "StoreManifest.xml";

		protected const string ConfigFileName = "config.xml";

		protected const string AppInsightsConfigFileName = "ApplicationInsights.config";

		protected const string XboxServicesConfigFileName = "xboxservices.config";

		protected const string AppxManifestName = "AppxManifest.xml";

		protected const string DefaultLanguage = "en-US";

		private const string ApplicationIdPrefix = "aow";

		private const string AppNameResName = "AppName";

		private const string AppxResourcesFileName = "resources.pri";

		private IPackageInformation packInfo;

		public PackageObjectDefaults PackageObjectDefaults { get; private set; }

		protected ApkObjectModel ApkModel { get; private set; }

		protected IReadOnlyCollection<AppxPackageConfiguration> PackageConfigs { get; private set; }

		protected IPortableRepositoryHandler Repository { get; private set; }

		public PortableApkToAppxConverter(ApkObjectModel apkObjectModel, IPortableRepositoryHandler repositoryHandler, IReadOnlyCollection<AppxPackageConfiguration> packageConfigs, IPackageInformation info)
			: this(apkObjectModel, repositoryHandler, packageConfigs, info, null)
		{
		}

		public PortableApkToAppxConverter(ApkObjectModel apkObjectModel, IPortableRepositoryHandler repositoryHandler, IReadOnlyCollection<AppxPackageConfiguration> packageConfigs, IPackageInformation info, PackageObjectDefaults packageObjectDefaults)
		{
			if (apkObjectModel == null)
			{
				throw new ArgumentNullException("apkObjectModel");
			}
			if (info == null)
			{
				throw new ArgumentNullException("info");
			}
			if (packageConfigs == null || packageConfigs.Count == 0)
			{
				throw new ArgumentException("Package configure list is NULL or empty", "packageConfigs");
			}
			ApkModel = apkObjectModel;
			Repository = repositoryHandler;
			PackageConfigs = packageConfigs;
			packInfo = info;
			PackageObjectDefaults = packageObjectDefaults;
		}

		[SuppressMessage("Microsoft.Globalization", "CA1308:NormalizeStringsToUppercase", Justification = "Need processor architecture in lower case")]
		public void GenerateOneAppxDirectory(AppxPackageConfiguration packageConfig)
		{
			if (!PortableUtilsServiceLocator.Initialized)
			{
				throw new InvalidOperationException("Portable utilities not initialized.");
			}
			if (packageConfig == null)
			{
				throw new ArgumentNullException("packageConfig");
			}
			PopulateImportantPackageInformationFromManifestMetadata();
			if (ApkModel.ManifestInfo.Application.Icon == null)
			{
				ApkModel.InjectTestIconResource(PackageObjectDefaults.ApplicationIconResourceId, PackageObjectDefaults.ApplicationIconFilePath);
			}
			CopyAppxBoilerplate(packageConfig);
			string appxProjectRootFolder = Repository.GetAppxProjectRootFolder(packageConfig);
			string text = PortableUtilsServiceLocator.FileUtils.PathCombine(appxProjectRootFolder, "AppxManifest.xml");
			LoggerCore.Log("Manifest File path is {0}", text);
			ManifestWriter manifestWriter = new ManifestWriter(packageConfig.PackageType, text, text);
			LoggerCore.Log("Project Root Folder path {0}", appxProjectRootFolder);
			StringsWriter stringsWriter = new StringsWriter(appxProjectRootFolder);
			AssetsWriter assetsWriter = new AssetsWriter(appxProjectRootFolder);
			WritePackageName(manifestWriter);
			WritePackagePublisher(manifestWriter);
			WritePackagePublisherDisplayName(manifestWriter);
			WriteApplicationId(manifestWriter);
			manifestWriter.ApkName = Path.GetFileName(Repository.RetrievePackageFilePath());
			WriteVersion(manifestWriter);
			manifestWriter.ProcessorArchitecture = packageConfig.PackageArch.ToString().ToLowerInvariant();
			WriteAppName(manifestWriter, stringsWriter);
			foreach (string allLanguageQualifier in stringsWriter.AllLanguageQualifiers)
			{
				LoggerCore.Log("Adding Language Qualifier {0}", allLanguageQualifier);
				manifestWriter.AddLanguage(allLanguageQualifier);
			}
			manifestWriter.AddLanguage("en-US");
			WriteCapabilites(manifestWriter);
			WriteShareTargetExtension(manifestWriter);
			WriteFileTypeAssociationExtension(manifestWriter);
			using (ImageAssetsConverter imageAssetsConverter = new ImageAssetsConverter(ApkModel, packageConfig.PackageType, manifestWriter, assetsWriter, Repository, PackageObjectDefaults))
			{
				imageAssetsConverter.WriteImageAssets();
				WriteBackgroundColor(manifestWriter, imageAssetsConverter.CalculatedBackgroundColor);
			}
			WriteInitialScreenOrientation(packageConfig.PackageType, manifestWriter);
			WritePackageProtocolExtension(manifestWriter);
			WriteBackgroundTaskExtension(manifestWriter);
			manifestWriter.WriteToFile();
			stringsWriter.WriteReswFiles();
			WriteConfigFile(PortableUtilsServiceLocator.FileUtils.PathCombine(appxProjectRootFolder, "config.xml"));
			ConvertMicrosoftServicesConfig(appxProjectRootFolder);
			WriteWindowsStoreProxyFile(PortableUtilsServiceLocator.FileUtils.PathCombine(appxProjectRootFolder, "WindowsStoreProxy.xml"));
			GeneratePriFiles(appxProjectRootFolder, manifestWriter);
			stringsWriter.CleanupAllReswFiles();
		}

		protected static void WriteStoreManifestFile(string storeManifestFilePath)
		{
			StoreManifestWriter storeManifestWriter = new StoreManifestWriter(storeManifestFilePath);
			storeManifestWriter.WriteToFile();
		}

		protected static void WriteFileTypeAssociationExtension(ManifestWriter manifestWriter)
		{
			if (manifestWriter == null)
			{
				throw new ArgumentNullException("manifestWriter");
			}
		}

		protected static void WriteShareTargetExtensionHelper(ManifestWriter manifestWriter, IReadOnlyList<ManifestIntentFilter> filters)
		{
			if (manifestWriter == null)
			{
				throw new ArgumentNullException("manifestWriter");
			}
			if (filters == null)
			{
				throw new ArgumentNullException("filters");
			}
			foreach (ManifestIntentFilter filter in filters)
			{
				foreach (string action in filter.Actions)
				{
					if (string.Compare(action, "android.intent.action.SEND", StringComparison.Ordinal) != 0 && string.Compare(action, "android.intent.action.SENDTO", StringComparison.Ordinal) != 0 && string.Compare(action, "android.intent.action.SEND_MULTIPLE", StringComparison.Ordinal) != 0)
					{
						continue;
					}
					foreach (ManifestIntentFilterData datum in filter.Data)
					{
						if (!string.IsNullOrWhiteSpace(datum.MimeType))
						{
							LoggerCore.Log("Action: {0}, MimeType: {1}.", action, datum.MimeType);
							if (string.Compare(datum.MimeType, "*/*", StringComparison.OrdinalIgnoreCase) == 0)
							{
								manifestWriter.AddShareTargetDataFormat("Html");
								manifestWriter.AddShareTargetDataFormat("Text");
								manifestWriter.AddShareTargetDataFormat("Uri");
								manifestWriter.AddShareTargetDataFormat("Bitmap");
							}
							else if (string.Compare(datum.MimeType, "text/html", StringComparison.OrdinalIgnoreCase) == 0)
							{
								manifestWriter.AddShareTargetDataFormat("Html");
							}
							else if (string.Compare(datum.MimeType, "text/plain", StringComparison.OrdinalIgnoreCase) == 0)
							{
								manifestWriter.AddShareTargetDataFormat("Text");
								manifestWriter.AddShareTargetDataFormat("Uri");
							}
							else if (datum.MimeType.StartsWith("text/", StringComparison.OrdinalIgnoreCase))
							{
								manifestWriter.AddShareTargetDataFormat("Html");
								manifestWriter.AddShareTargetDataFormat("Text");
								manifestWriter.AddShareTargetDataFormat("Uri");
							}
							else if (datum.MimeType.StartsWith("image/", StringComparison.OrdinalIgnoreCase))
							{
								manifestWriter.AddShareTargetDataFormat("Bitmap");
							}
							else
							{
								LoggerCore.Log("Mime Type {0} for the action {1} is not known.", datum.MimeType, action);
							}
						}
						else
						{
							LoggerCore.Log("We ignore action of {0} with empty mimetype.", action);
						}
					}
				}
			}
		}

		protected void CopyAppxBoilerplate(AppxPackageConfiguration packageConfig)
		{
			string appxProjectRootFolder = Repository.GetAppxProjectRootFolder(packageConfig);
			string appxEntryAppTemplatePath = Repository.GetAppxEntryAppTemplatePath(packageConfig);
			PortableUtilsServiceLocator.FileUtils.RecursivelyCopyDirectory(appxEntryAppTemplatePath, appxProjectRootFolder);
			string text = Repository.RetrievePackageFilePath();
			string fileName = Path.GetFileName(text);
			string destination = Path.Combine(new string[2] { appxProjectRootFolder, fileName });
			PortableUtilsServiceLocator.FileUtils.CopyFile(text, destination, overwrite: true);
		}

		protected void WriteApplicationId(ManifestWriter manifestWriter)
		{
			if (manifestWriter == null)
			{
				throw new ArgumentNullException("manifestWriter");
			}
			if (!string.IsNullOrWhiteSpace(packInfo.PackageIdentityName))
			{
				if (DoesPackageNameHaveUnsupportedChars(packInfo.PackageIdentityName))
				{
					throw new ConverterException(string.Format(CultureInfo.InvariantCulture, "SPACE or Underscore not allowed in Package Identity name. Provided identity name is {0}", new object[1] { packInfo.PackageIdentityName }));
				}
				string input = "aow" + packInfo.PackageIdentityName.Replace(".", ".aow");
				string pattern = "([A-Za-z][A-Za-z0-9]*)(\\.[A-Za-z][A-Za-z0-9]*)*";
				Match match = Regex.Matches(input, pattern)[0];
				manifestWriter.ApplicationId = match.ToString();
				LoggerCore.Log("Application Id: {0}", manifestWriter.ApplicationId);
			}
		}

		protected void WriteAppName(ManifestWriter manifestWriter, StringsWriter stringsWriter)
		{
			if (manifestWriter == null)
			{
				throw new ArgumentNullException("manifestWriter");
			}
			if (stringsWriter == null)
			{
				throw new ArgumentNullException("stringsWriter");
			}
			ManifestStringResource manifestStringResource = null;
			manifestStringResource = ((ApkModel.ManifestInfo.Application.Label != null || PackageObjectDefaults == null) ? ApkModel.ManifestInfo.Application.Label : PackageObjectDefaults.ApplicationNameResource);
			manifestWriter.AppName = ExtractResourceValue(manifestStringResource, stringsWriter);
			LoggerCore.Log("Application Name: {0}", manifestWriter.AppName);
		}

		protected void WriteBackgroundColor(ManifestWriter manifestWriter, Color? calculatedBackgroundColor)
		{
			if (manifestWriter == null)
			{
				throw new ArgumentNullException("manifestWriter");
			}
			if (ApkModel.ManifestInfo.Application.BackgroundColorData != null)
			{
				ManifestStringResource value = ApkModel.ManifestInfo.Application.BackgroundColorData.Value;
				manifestWriter.BackgroundColor = value.Content;
			}
			else if (calculatedBackgroundColor.HasValue)
			{
				if (calculatedBackgroundColor == Color.Transparent)
				{
					manifestWriter.BackgroundColor = "transparent";
				}
				else
				{
					manifestWriter.BackgroundColor = calculatedBackgroundColor.ToString();
				}
			}
			else
			{
				manifestWriter.BackgroundColor = "#000000";
			}
			LoggerCore.Log("Background Color: {0}", manifestWriter.BackgroundColor);
		}

		protected void WriteCapabilites(ManifestWriter writer)
		{
			if (writer == null)
			{
				throw new ArgumentNullException("writer");
			}
			foreach (ManifestUsesPermission usesPermission in ApkModel.ManifestInfo.UsesPermissions)
			{
				PermissionMapItem permissionMapItem = PermissionMap.MapPermission(usesPermission.Name.Content);
				if (permissionMapItem.PermissionType == PermissionType.Present)
				{
					if (permissionMapItem.MappedCapabilities == null)
					{
						continue;
					}
					foreach (AppxCapability mappedCapability in permissionMapItem.MappedCapabilities)
					{
						if (mappedCapability.CapabilityType == CapabilityType.Software)
						{
							writer.AddSoftwareCapability(mappedCapability.CapabilityName);
						}
						else
						{
							writer.AddDeviceCapability(mappedCapability.CapabilityName);
						}
					}
				}
				else
				{
					LoggerCore.Log("{0} is not mapped because no mapping is present.", usesPermission.Name.Content);
				}
			}
		}

		protected void WriteConfigFile(string configFilePath)
		{
			ConfigWriter configWriter = new ConfigWriter(configFilePath);
			configWriter.AndroidPackageId = GetRawString(ApkModel.ManifestInfo.PackageNameResource);
			configWriter.WriteToFile();
		}

		protected void ConvertMicrosoftServicesConfig(string projectRootFolderPath)
		{
			if (ApkModel.ApkConfigFile != null && ApkModel.ApkConfigFile.Analytics != null)
			{
				string outputFilePath = PortableUtilsServiceLocator.FileUtils.PathCombine(projectRootFolderPath, "ApplicationInsights.config");
				string analyticsKey = ApkModel.ApkConfigFile.Analytics.AnalyticsKey;
				if (!string.IsNullOrEmpty(analyticsKey))
				{
					AppInsightsConfigWriter appInsightsConfigWriter = new AppInsightsConfigWriter(outputFilePath);
					appInsightsConfigWriter.InstrumentationKey = analyticsKey;
					appInsightsConfigWriter.WriteToFile();
				}
			}
			if (ApkModel.ApkConfigFile != null && ApkModel.ApkConfigFile.GameServices != null)
			{
				string outputFilePath2 = PortableUtilsServiceLocator.FileUtils.PathCombine(projectRootFolderPath, "xboxservices.config");
				GameServicesConfigWriter gameServicesConfigWriter = new GameServicesConfigWriter(ApkModel.ApkConfigFile.GameServices);
				gameServicesConfigWriter.WriteToFile(outputFilePath2);
			}
		}

		protected void WriteWindowsStoreProxyFile(string destinationWindowsProxyFile)
		{
			string text = PortableZipUtils.ExtractFileFromZip(Repository.RetrievePackageFilePath(), "res/raw/WindowsStoreProxy.xml", Repository.RetrievePackageExtractionPath());
			if (text != null)
			{
				LoggerCore.Log("Found WindowsStoreProxy.xml file, copying over to appx side");
				PortableUtilsServiceLocator.FileUtils.CopyFile(text, destinationWindowsProxyFile, overwrite: true);
			}
		}

		protected void WriteInitialScreenOrientation(AppxPackageType appxType, ManifestWriter manifestWriter)
		{
			if (manifestWriter == null)
			{
				throw new ArgumentNullException("manifestWriter");
			}
			IReadOnlyList<ManifestActivity> activities = ApkModel.ManifestInfo.Application.Activities;
			ICollection<ManifestActivity> collection = new List<ManifestActivity>();
			ApkScreenOrientationType mainActivityOrientation = ApkScreenOrientationType.Undeclared;
			foreach (ManifestActivity item in activities)
			{
				if (item.HasMainActivity)
				{
					mainActivityOrientation = item.ScreenOrientation;
				}
				else if (item.ScreenOrientation != ApkScreenOrientationType.Undeclared)
				{
					collection.Add(item);
				}
			}
			HashSet<AppxScreenOrientationType> initialScreenOrientations = GetInitialScreenOrientations(mainActivityOrientation, collection);
			foreach (AppxScreenOrientationType item2 in initialScreenOrientations)
			{
				manifestWriter.AddInitialScreenOrientations(item2);
			}
		}

		protected void WritePackageName(ManifestWriter manifestWriter)
		{
			if (manifestWriter == null)
			{
				throw new ArgumentNullException("manifestWriter");
			}
			if (!string.IsNullOrWhiteSpace(packInfo.PackageIdentityName))
			{
				if (DoesPackageNameHaveUnsupportedChars(packInfo.PackageIdentityName))
				{
					throw new ConverterException("SPACE or Underscore not allowed in Package Identity name");
				}
				manifestWriter.PackageIdentityName = packInfo.PackageIdentityName;
			}
			LoggerCore.Log("Package Name: {0}", manifestWriter.PackageIdentityName);
		}

		protected void WritePackagePublisherDisplayName(ManifestWriter manifestWriter)
		{
			if (manifestWriter == null)
			{
				throw new ArgumentNullException("manifestWriter");
			}
			if (!string.IsNullOrWhiteSpace(packInfo.PackagePublisherDisplayName))
			{
				manifestWriter.PackagePublisherDisplayName = packInfo.PackagePublisherDisplayName;
			}
			LoggerCore.Log("Publisher Display Name: {0}", manifestWriter.PackagePublisherDisplayName);
		}

		protected void WritePackagePublisher(ManifestWriter manifestWriter)
		{
			if (manifestWriter == null)
			{
				throw new ArgumentNullException("manifestWriter");
			}
			if (!string.IsNullOrWhiteSpace(packInfo.PackagePublisher))
			{
				if (!IsValidPublisherName(packInfo.PackagePublisher))
				{
					throw new ConverterException("Package Publisher Name is not well formed.");
				}
				manifestWriter.PackagePublisher = packInfo.PackagePublisher;
			}
			LoggerCore.Log("Package Publisher: {0}", manifestWriter.PackagePublisher);
		}

		protected void WriteShareTargetExtension(ManifestWriter manifestWriter)
		{
			foreach (ManifestActivity activity in ApkModel.ManifestInfo.Application.Activities)
			{
				WriteShareTargetExtensionHelper(manifestWriter, activity.Filters);
			}
			foreach (ManifestActivityAlias activityAlias in ApkModel.ManifestInfo.Application.ActivityAliases)
			{
				WriteShareTargetExtensionHelper(manifestWriter, activityAlias.Filters);
			}
		}

		[SuppressMessage("Microsoft.Globalization", "CA1308:NormalizeStringsToUppercase", Justification = "Matching APPX manifest.")]
		protected void WritePackageProtocolExtension(ManifestWriter manifestWriter)
		{
			if (manifestWriter == null)
			{
				throw new ArgumentNullException("manifestWriter");
			}
			string rawString = GetRawString(ApkModel.ManifestInfo.PackageNameResource);
			byte[] bytes = Encoding.UTF8.GetBytes(rawString.ToLowerInvariant());
			string protocolName = "a+" + CryptoHelper.ComputeMD5HashAsHexadecimal(bytes).ToLowerInvariant();
			manifestWriter.AddProtocolExtension(protocolName, "optional");
		}

		protected void WriteBackgroundTaskExtension(ManifestWriter manifestWriter)
		{
			if (manifestWriter == null)
			{
				throw new ArgumentNullException("manifestWriter");
			}
			LoggerCore.Log("Adding system event background task.");
			manifestWriter.AddBackgroundTaskExtension("BackgroundTask.MainTask", "Arcadia.exe", "systemEvent");
			LoggerCore.Log("Adding general event background task.");
			manifestWriter.AddBackgroundTaskExtension("BackgroundTask.MainTask", "Arcadia.exe", "general");
			if (ApkModel.ManifestInfo.HasPermission("com.google.android.c2dm.permission.RECEIVE") || ApkModel.ManifestInfo.HasPermission("com.microsoft.services.pushnotification.permission.RECEIVE"))
			{
				LoggerCore.Log("Adding push notification background task.");
				manifestWriter.AddBackgroundTaskExtension("BackgroundTask.MainTask", "Arcadia.exe", "pushNotification");
			}
			if (ApkModel.ManifestInfo.HasPermission("android.permission.ACCESS_FINE_LOCATION"))
			{
				LoggerCore.Log("Adding location background task.");
				manifestWriter.AddBackgroundTaskExtension("BackgroundTask.MainTask", "Arcadia.exe", "location");
			}
			LoggerCore.Log("Adding background task for {0}.", "BackgroundTask.OutOfProcTask");
			manifestWriter.AddBackgroundTaskExtension("BackgroundTask.OutOfProcTask", null, "general");
			LoggerCore.Log("Adding in process server for {0}.", "BackgroundTask.MainTask");
			manifestWriter.AddInProcessServerExtension("AoWBackgroundTask.dll", "BackgroundTask.MainTask", "both");
			LoggerCore.Log("Adding in process server for {0}.", "BackgroundTask.OutOfProcTask");
			manifestWriter.AddInProcessServerExtension("AoWBackgroundTask.dll", "BackgroundTask.OutOfProcTask", "both");
			LoggerCore.Log("Adding restricted capabilities for graphics improvements.");
			manifestWriter.AddRestrictedCapability("previewUiComposition");
			LoggerCore.Log("Adding restricted capabilities for graphics improvements.");
			manifestWriter.AddRestrictedCapability("previewUiComposition");
		}

		protected void WriteVersion(ManifestWriter manifestWriter)
		{
			if (manifestWriter == null)
			{
				throw new ArgumentNullException("manifestWriter");
			}
			string rawString = GetRawString(ApkModel.ManifestInfo.VersionCode);
			uint result = 0u;
			if (!uint.TryParse(rawString, out result))
			{
				throw new ArgumentException("Invalid version code from APK: " + result);
			}
			manifestWriter.ApkVersion = rawString;
			uint num = 0u;
			uint num2 = 0u;
			num = (result >> 16) & 0xFFFFu;
			num2 = result & 0xFFFFu;
			manifestWriter.AppxVersion = string.Format(CultureInfo.InvariantCulture, "{0}.{1}.{2}.{3}", num, num2, 0u, 0u);
			LoggerCore.Log("APK Version: {0} --> APPX Version: {1}", manifestWriter.ApkVersion, manifestWriter.AppxVersion);
		}

		protected void GeneratePriFiles(string projectRootFolderPath, ManifestWriter manifestWriter)
		{
			if (string.IsNullOrEmpty(projectRootFolderPath))
			{
				throw new ArgumentException("Must not be null or empty.", "projectRootFolderPath");
			}
			if (manifestWriter == null)
			{
				throw new ArgumentNullException("manifestWriter");
			}
			PackageResourceIndexMaker packageResourceIndexMaker = new PackageResourceIndexMaker(Repository.RetrieveMakePriToolPath(), manifestWriter.PackageIdentityName, "en-US");
			packageResourceIndexMaker.Run(Repository.RetrieveMakePriConfigFilePath(), projectRootFolderPath, Path.Combine(new string[2] { projectRootFolderPath, "resources.pri" }));
		}

		protected void PopulateImportantPackageInformationFromManifestMetadata()
		{
			if (ApkModel == null || ApkModel.ManifestInfo == null || ApkModel.ManifestInfo.Application == null || ApkModel.ManifestInfo.Application.MetadataElements == null)
			{
				return;
			}
			foreach (ManifestApplicationMetadata metadataElement in ApkModel.ManifestInfo.Application.MetadataElements)
			{
				if (metadataElement.Value != null && !string.IsNullOrWhiteSpace(metadataElement.Value.Content) && !metadataElement.Value.IsResource)
				{
					if (metadataElement.Name.Equals("com.microsoft.windows.package.identity.name", StringComparison.OrdinalIgnoreCase))
					{
						packInfo.PackageIdentityName = metadataElement.Value.Content;
						LoggerCore.Log("Package Identity from APK Meta-data {0}", packInfo.PackageIdentityName);
					}
					else if (metadataElement.Name.Equals("com.microsoft.windows.package.identity.publisher", StringComparison.OrdinalIgnoreCase))
					{
						packInfo.PackagePublisher = metadataElement.Value.Content;
						LoggerCore.Log("Package publisher from APK Meta-data {0}", packInfo.PackagePublisher);
					}
					else if (metadataElement.Name.Equals("com.microsoft.windows.package.properties.publisherdisplayname", StringComparison.OrdinalIgnoreCase))
					{
						packInfo.PackagePublisherDisplayName = metadataElement.Value.Content;
						LoggerCore.Log("Package publisher display name from APK Meta-data {0}", packInfo.PackagePublisherDisplayName);
					}
				}
				else
				{
					LoggerCore.Log("Provided Meta-data {0}'s value is either NULL or has been localized. This is not a supported scenario.", metadataElement.Name);
				}
			}
		}

		private static bool DoesPackageNameHaveUnsupportedChars(string apkPackageName)
		{
			Regex regex = new Regex("[_ ]+");
			return regex.IsMatch(apkPackageName);
		}

		private static HashSet<AppxScreenOrientationType> GetInitialScreenOrientations(ApkScreenOrientationType mainActivityOrientation, ICollection<ManifestActivity> supportingActivityList)
		{
			if (ScreenOrientationMap.GetContradictionRotatingActivites(mainActivityOrientation, supportingActivityList).Count > 0)
			{
				LoggerCore.Log("No APPX rotation will be enforced since there are contradicting screen orientation.");
				return new HashSet<AppxScreenOrientationType>();
			}
			ScreenOrientationItem screenOrientationItem = ScreenOrientationMap.MapActivityOrientation(mainActivityOrientation);
			if (screenOrientationItem == null)
			{
				LoggerCore.Log("No APPX rotation will be enforced since the main activity's orientation is not found.");
				return new HashSet<AppxScreenOrientationType>();
			}
			return screenOrientationItem.PossibleScreenOrientationTypes;
		}

		private static string GetRawString(ManifestStringResource manifestValue)
		{
			if (manifestValue.IsResource)
			{
				throw new ConverterException("The field isn't expected be a reference to resource");
			}
			if (string.IsNullOrEmpty(manifestValue.Content))
			{
				throw new ConverterException("The field must be an non-empty string");
			}
			return manifestValue.Content;
		}

		private static bool IsValidPublisherName(string publisherName)
		{
			string pattern = "(CN|L|O|OU|E|C|S|STREET|T|G|I|SN|DC|SERIALNUMBER|(OID\\.(0|[1-9][0-9]*)(\\.(0|[1-9][0-9]*))+))=(([^,+=\"<>#;])+|\".*\")(, ((CN|L|O|OU|E|C|S|STREET|T|G|I|SN|DC|SERIALNUMBER|(OID\\.(0|[1-9][0-9]*)(\\.(0|[1-9][0-9]*))+))=(([^,+=\"<>#;])+|\".*\")))*";
			Regex regex = new Regex(pattern);
			return regex.IsMatch(publisherName);
		}

		private static string MakeResStringReference(string resName)
		{
			return "ms-resource:" + resName;
		}

		private string ExtractResourceValue(ManifestStringResource resource, StringsWriter stringsWriter)
		{
			if (resource.IsResource)
			{
				ApkResource resource2 = ApkResourceHelper.GetResource(resource, ApkModel.Resources);
				_ = CultureInfo.CurrentCulture.Name;
				foreach (ApkResourceValue value in resource2.Values)
				{
					if (stringsWriter != null && !string.IsNullOrWhiteSpace(value.Value))
					{
						stringsWriter.AddString("AppName", value.Value, value.Config.Locale);
					}
				}
				return MakeResStringReference("AppName");
			}
			return resource.Content;
		}
	}
}