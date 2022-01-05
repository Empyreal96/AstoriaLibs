using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using Microsoft.Arcadia.Marketplace.PackageObjectModel;
using Microsoft.Arcadia.Marketplace.PackageObjectModel.Portable.Appx;
using Microsoft.Arcadia.Marketplace.Utils.Log;
using Microsoft.Arcadia.Marketplace.Utils.Parsers;
using Microsoft.Arcadia.Marketplace.Utils.Portable;
using Microsoft.Arcadia.Marketplace.Utils.Xml;

namespace Microsoft.Arcadia.Marketplace.Converter
{
	public sealed class ManifestWriter
	{
		private class InProcessServerClassInfo
		{
			public string ClassId { get; private set; }

			public string ThreadingModel { get; private set; }

			public InProcessServerClassInfo(string classId, string threadingModel)
			{
				ClassId = classId;
				ThreadingModel = threadingModel;
			}
		}

		private class BackgroundTaskInfo
		{
			public string Executable { get; private set; }

			public HashSet<string> TaskTypes { get; private set; }

			public BackgroundTaskInfo(string executable)
			{
				Executable = executable;
				TaskTypes = new HashSet<string>();
			}
		}

		private class FieldWriteInfo
		{
			public string ElementPath { get; private set; }

			public string AttributeName { get; private set; }

			public string Value { get; private set; }

			public FieldWriteInfo(string elementPath, string attributeName, string value)
			{
				ElementPath = elementPath;
				AttributeName = attributeName;
				Value = value;
			}
		}

		private const string AppxUapPrefix = "uap";

		private const string AppxMobilePrefix = "mobile";

		private const string RestrictedCapabilitiesPrefix = "rescap";

		private const int ProtocolPrefixMaxLength = 39;

		private static HashSet<string> uapCapabilities = new HashSet<string> { "documentsLibrary", "picturesLibrary", "videosLibrary", "musicLibrary", "enterpriseAuthentication", "sharedUserCertificates", "removableStorage", "appointments", "contacts", "phoneCall" };

		private AppxPackageType appxType;

		private string templateFilePath;

		private string outputFilePath;

		private HashSet<string> languages;

		private HashSet<string> shareTargetDataFormats;

		private Dictionary<string, string> protocolExtensions;

		private HashSet<string> softwareCapabilities;

		private HashSet<string> deviceCapabilities;

		private HashSet<string> restrictedCapabilities;

		private HashSet<AppxScreenOrientationType> initialScreenOrientations;

		private List<FileTypeAssociation> fileTypesExtensions;

		private Dictionary<string, BackgroundTaskInfo> backgroundTaskExtensions;

		private Dictionary<string, List<InProcessServerClassInfo>> inProcessServerExtensions;

		public string PackageIdentityName { get; set; }

		public string PackagePhoneIdentity { get; set; }

		public string ApplicationId { get; set; }

		public string PackagePublisher { get; set; }

		public string PackagePublisherDisplayName { get; set; }

		public string ApkName { get; set; }

		public string ApkVersion { get; set; }

		public string AppxVersion { get; set; }

		public string ProcessorArchitecture { get; set; }

		public string AppName { get; set; }

		public string StoreLogo { get; set; }

		public string AppLogo { get; set; }

		public string TileLogoSmall { get; set; }

		public string TileLogoMedium { get; set; }

		public string TileLogoWide { get; set; }

		public string TileLogoLarge { get; set; }

		public string SplashScreen { get; set; }

		public string BackgroundColor { get; set; }

		public ManifestWriter(AppxPackageType type, string templateFilePath, string outputFilePath)
		{
			if (string.IsNullOrWhiteSpace(templateFilePath))
			{
				throw new ArgumentException("templateFilePath is null or empty");
			}
			if (string.IsNullOrWhiteSpace(outputFilePath))
			{
				throw new ArgumentException("manifestFilePath is null or empty");
			}
			appxType = type;
			this.templateFilePath = templateFilePath;
			this.outputFilePath = outputFilePath;
			PackageIdentityName = (string.IsNullOrWhiteSpace(PackageIdentityName) ? "ClientPackageName" : PackageIdentityName);
			PackagePublisher = (string.IsNullOrWhiteSpace(PackagePublisher) ? "CN=developer" : PackagePublisher);
			PackagePublisherDisplayName = (string.IsNullOrWhiteSpace(PackagePublisherDisplayName) ? "DeveloperDisplayName" : PackagePublisherDisplayName);
			languages = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
			shareTargetDataFormats = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
			protocolExtensions = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
			softwareCapabilities = new HashSet<string>(StringComparer.Ordinal);
			deviceCapabilities = new HashSet<string>(StringComparer.Ordinal);
			restrictedCapabilities = new HashSet<string>(StringComparer.Ordinal);
			initialScreenOrientations = new HashSet<AppxScreenOrientationType>();
			fileTypesExtensions = new List<FileTypeAssociation>();
			backgroundTaskExtensions = new Dictionary<string, BackgroundTaskInfo>(StringComparer.Ordinal);
			inProcessServerExtensions = new Dictionary<string, List<InProcessServerClassInfo>>(StringComparer.Ordinal);
		}

		public void AddLanguage(string languageQualifier)
		{
			if (!LanguageQualifier.IsValidLanguageQualifier(languageQualifier))
			{
				string message = string.Format(CultureInfo.InvariantCulture, "Invalid Language Qualifier {0} or not supported in Windows.", new object[1] { languageQualifier });
				throw new ConverterException(message);
			}
			languages.Add(languageQualifier);
		}

		public void AddShareTargetDataFormat(string dataFormat)
		{
			if (string.IsNullOrWhiteSpace(dataFormat))
			{
				throw new ArgumentException("Data format shouldn't be null or empty", "dataFormat");
			}
			shareTargetDataFormats.Add(dataFormat);
		}

		public void AddProtocolExtension(string protocolName, string returnResults)
		{
			if (string.IsNullOrWhiteSpace(protocolName))
			{
				throw new ArgumentException("Protocol name shouldn't be null or empty", "protocolName");
			}
			if (protocolName.Length > 39)
			{
				throw new ArgumentNullException("Protocol name must be less than or equal to " + 39 + " character(s).");
			}
			protocolExtensions.Add(protocolName, returnResults);
		}

		public void AddFileTypeAssociation(FileTypeAssociation fileTypeAssocition)
		{
			fileTypesExtensions.Add(fileTypeAssocition);
		}

		public void AddSoftwareCapability(string softwareCapability)
		{
			if (string.IsNullOrWhiteSpace(softwareCapability))
			{
				throw new ArgumentException("softwareCapability must be provided.", "softwareCapability");
			}
			LoggerCore.Log("Adding software capability {0}", softwareCapability);
			softwareCapabilities.Add(softwareCapability);
		}

		public void AddDeviceCapability(string deviceCapability)
		{
			if (string.IsNullOrWhiteSpace(deviceCapability))
			{
				throw new ArgumentException("deviceCapability must be provided.", "deviceCapability");
			}
			LoggerCore.Log("Adding device capability {0}", deviceCapability);
			deviceCapabilities.Add(deviceCapability);
		}

		public void AddRestrictedCapability(string restrictedCapability)
		{
			if (string.IsNullOrWhiteSpace(restrictedCapability))
			{
				throw new ArgumentException("restrictedCapability must be provided.", "restrictedCapability");
			}
			LoggerCore.Log("Adding restricted capability {0}", restrictedCapability);
			restrictedCapabilities.Add(restrictedCapability);
		}

		public void AddInitialScreenOrientations(AppxScreenOrientationType initialScreenOrientation)
		{
			LoggerCore.Log("Adding initial screen orientation {0}", initialScreenOrientation);
			initialScreenOrientations.Add(initialScreenOrientation);
		}

		public void AddBackgroundTaskExtension(string entryPoint, string executable, string taskType)
		{
			if (string.IsNullOrWhiteSpace(entryPoint))
			{
				throw new ArgumentException("entryPoint must be provided", "entryPoint");
			}
			if (string.IsNullOrWhiteSpace(taskType))
			{
				throw new ArgumentException("taskType must be provided", "taskType");
			}
			if (!backgroundTaskExtensions.TryGetValue(entryPoint, out var value))
			{
				value = new BackgroundTaskInfo(executable);
				backgroundTaskExtensions.Add(entryPoint, value);
			}
			value.TaskTypes.Add(taskType);
		}

		public void AddInProcessServerExtension(string path, string activatableClassId, string threadingMode)
		{
			if (!inProcessServerExtensions.TryGetValue(path, out var value))
			{
				value = new List<InProcessServerClassInfo>();
				inProcessServerExtensions.Add(path, value);
			}
			value.Add(new InProcessServerClassInfo(activatableClassId, threadingMode));
		}

		public void WriteToFile()
		{
			LoggerCore.Log("Writing APPX manifest file to " + templateFilePath);
			LoggerCore.Log("APPX type is " + appxType);
			AppxManifestDefs appxManifestDefs = null;
			XmlDocWriter xmlDocWriter = new XmlDocWriter(templateFilePath, InputType.FilePath);
			AddNamespaces(xmlDocWriter);
			appxManifestDefs = new AppxManifestDefs(XmlConstants.XmlManifestDefaultPrefix, "mobile", "uap");
			WriteFields(xmlDocWriter, appxManifestDefs);
			WriteLanguages(xmlDocWriter, appxManifestDefs);
			VerifyExistingExtensions(xmlDocWriter, appxManifestDefs);
			InjectProjectAMobileExtensionSubelements(xmlDocWriter, appxManifestDefs);
			WriteShareTargetExtension(xmlDocWriter, appxManifestDefs);
			WriteProtocolExtensions(xmlDocWriter, appxManifestDefs);
			WriteFileTypeExtension(xmlDocWriter, appxManifestDefs);
			WriteBackgroundTaskExtensions(xmlDocWriter, appxManifestDefs);
			WriteCapabilities(xmlDocWriter, appxManifestDefs);
			WriteInProcessServerExtensions(xmlDocWriter, appxManifestDefs);
			xmlDocWriter.WriteToFile(outputFilePath);
			LoggerCore.Log("Finished writing APPX manifest file");
		}

		private static void VerifyExistingExtensions(XmlDocWriter writer, AppxManifestDefs manifestDefs)
		{
			if (writer.HasElement(manifestDefs.ExtensionsElementPath))
			{
				List<KeyValuePair<string, string>> list = new List<KeyValuePair<string, string>>();
				list.Add(new KeyValuePair<string, string>("Category", "windows.fileTypeAssociation"));
				list.Add(new KeyValuePair<string, string>("Category", "windows.shareTarget"));
				if (writer.QueryQualifyingChildElements(manifestDefs.ExtensionsElementPath, list))
				{
					throw new ConverterException("Extension elements for File Type Association or Share Target already exists.");
				}
			}
		}

		private static void WriteOneField(XmlDocWriter writer, FieldWriteInfo info)
		{
			if (string.IsNullOrWhiteSpace(info.Value))
			{
				throw new ArgumentException("The value to write is null or empty", "info");
			}
			LoggerCore.Log("Writing one field:");
			if (string.IsNullOrWhiteSpace(info.AttributeName))
			{
				LoggerCore.Log(info.ElementPath + "/[.] = " + info.Value);
				writer.SetElementInnerText(info.ElementPath, info.Value);
				return;
			}
			LoggerCore.Log(info.ElementPath + "/@" + info.AttributeName + " = " + info.Value);
			writer.SetElementAttribute(info.ElementPath, info.AttributeName, info.Value);
		}

		private static bool IsUapCapability(string capability)
		{
			return uapCapabilities.Contains(capability);
		}

		private void AddNamespaces(XmlDocWriter writer)
		{
			writer.AddDefaultNamespace(XmlConstants.XmlManifestDefaultPrefix, "http://schemas.microsoft.com/appx/manifest/foundation/windows10");
			writer.AddNamespace("uap", "http://schemas.microsoft.com/appx/manifest/uap/windows10");
			writer.AddNamespace("mobile", "http://schemas.microsoft.com/appx/manifest/mobile/windows10");
			if (restrictedCapabilities.Count > 0)
			{
				writer.AddNamespace("rescap", "http://schemas.microsoft.com/appx/manifest/foundation/windows10/restrictedcapabilities");
			}
		}

		private void InjectProjectAMobileExtensionSubelements(XmlDocWriter writer, AppxManifestDefs manifestDefs)
		{
			LoggerCore.Log("Writing mobile:projectA payload name and version fields");
			string text = manifestDefs.MobileProjectAExtensionElementPath + "/" + XmlUtilites.MakeElementPath("mobile", manifestDefs.PayloadNameElementName);
			writer.SetElementInnerText(text, ApkName);
			LoggerCore.Log("Payload Name: {0} - {1}", text, ApkName);
			string text2 = manifestDefs.MobileProjectAExtensionElementPath + "/" + XmlUtilites.MakeElementPath("mobile", manifestDefs.PayloadVersionElementName);
			writer.SetElementInnerText(text2, ApkVersion);
			LoggerCore.Log("Payload Version: {0} - {1}", text2, ApkVersion);
		}

		private void WriteFileTypeExtension(XmlDocWriter writer, AppxManifestDefs manifestDefs)
		{
			if (fileTypesExtensions == null || fileTypesExtensions.Count == 0)
			{
				LoggerCore.Log("No file type association extension to be injected into the xml.");
				return;
			}
			if (!writer.HasElement(manifestDefs.ExtensionsElementPath))
			{
				writer.AddChildElement(manifestDefs.ApplicationPath, null, manifestDefs.ExtensionsElementName, null, null);
			}
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			dictionary[manifestDefs.CategoryAttributeName] = manifestDefs.FileTypeTypeAssociateCategory;
			writer.AddChildElement(manifestDefs.ExtensionsElementPath, "uap", manifestDefs.ExtensionElementName, dictionary, null);
			foreach (FileTypeAssociation fileTypesExtension in fileTypesExtensions)
			{
				dictionary = new Dictionary<string, string>();
				dictionary[manifestDefs.NameAttribute] = fileTypesExtension.Name;
				string parentPath = manifestDefs.UapExtensionElementPath + string.Format(CultureInfo.InvariantCulture, "[@{0}='{1}']", new object[2] { manifestDefs.CategoryAttributeName, manifestDefs.FileTypeTypeAssociateCategory });
				writer.AddChildElement(parentPath, "uap", manifestDefs.FileTypeAssociationElementName, dictionary, null);
				string text = manifestDefs.FileTypeAssociationElementPath + string.Format(CultureInfo.InvariantCulture, "[@{0}='{1}']", new object[2] { manifestDefs.NameAttribute, fileTypesExtension.Name });
				writer.AddChildElement(text, "uap", manifestDefs.SupportedFileTypesElementName, null, null);
				string text2 = text + "/" + XmlUtilites.MakeElementPath("uap", manifestDefs.SupportedFileTypesElementName);
				writer.AddChildElement(text2, "uap", manifestDefs.FileTypeElementName, null, null);
				string elementPath = text2 + "/" + XmlUtilites.MakeElementPath("uap", manifestDefs.FileTypeElementName);
				writer.SetElementInnerText(elementPath, fileTypesExtension.FileType);
			}
		}

		private void WriteFields(XmlDocWriter writer, AppxManifestDefs manifestDefs)
		{
			WriteRequiredFields(writer, manifestDefs);
			WriteOptionalPackageRelatedFields(writer, manifestDefs);
			WriteOptionalFieldsInDefaultTile(writer, manifestDefs);
			WriteOptionalFieldsInSplashScreen(writer, manifestDefs);
			WriteOptionalInitialRotationPreference(writer, manifestDefs);
		}

		private void WriteOptionalPackageRelatedFields(XmlDocWriter writer, AppxManifestDefs manifestDefs)
		{
			FieldWriteInfo[] array = new FieldWriteInfo[4]
			{
			new FieldWriteInfo(manifestDefs.IdentityPath, manifestDefs.NameAttribute, PackageIdentityName),
			new FieldWriteInfo(manifestDefs.IdentityPath, manifestDefs.PackagePublisherAttribute, PackagePublisher),
			new FieldWriteInfo(manifestDefs.PropertiesPublisherDisplayNamePath, null, PackagePublisherDisplayName),
			new FieldWriteInfo(manifestDefs.ApplicationPath, manifestDefs.ApplicationIdAttribute, ApplicationId)
			};
			FieldWriteInfo[] array2 = array;
			foreach (FieldWriteInfo fieldWriteInfo in array2)
			{
				if (!string.IsNullOrWhiteSpace(fieldWriteInfo.Value))
				{
					WriteOneField(writer, fieldWriteInfo);
				}
			}
		}

		private void WriteRequiredFields(XmlDocWriter writer, AppxManifestDefs manifestDefs)
		{
			FieldWriteInfo[] array = new FieldWriteInfo[8]
			{
			new FieldWriteInfo(manifestDefs.IdentityPath, manifestDefs.VersionAttribute, AppxVersion),
			new FieldWriteInfo(manifestDefs.IdentityPath, manifestDefs.ProcessorArchitectureAttribute, ProcessorArchitecture),
			new FieldWriteInfo(manifestDefs.PropertiesDisplayNamePath, null, AppName),
			new FieldWriteInfo(manifestDefs.VisualElementsPath, manifestDefs.DisplayNameAttribute, AppName),
			new FieldWriteInfo(manifestDefs.PropertiesLogoPath, null, StoreLogo),
			new FieldWriteInfo(manifestDefs.VisualElementsPath, manifestDefs.AppLogoAttribute, AppLogo),
			new FieldWriteInfo(manifestDefs.VisualElementsPath, manifestDefs.MediumTileLogoAttribute, TileLogoMedium),
			new FieldWriteInfo(manifestDefs.VisualElementsPath, manifestDefs.BackgroundColorAttributeName, BackgroundColor)
			};
			FieldWriteInfo[] array2 = array;
			foreach (FieldWriteInfo fieldWriteInfo in array2)
			{
				if (string.IsNullOrWhiteSpace(fieldWriteInfo.Value))
				{
					throw new ConverterException("Required field hasn't been set");
				}
				WriteOneField(writer, fieldWriteInfo);
			}
		}

		private void WriteOptionalFieldsInDefaultTile(XmlDocWriter writer, AppxManifestDefs manifestDefs)
		{
			List<FieldWriteInfo> list = new List<FieldWriteInfo>();
			if (!string.IsNullOrWhiteSpace(TileLogoSmall))
			{
				FieldWriteInfo item = new FieldWriteInfo(manifestDefs.DefaultTilePath, manifestDefs.SmallTileLogoAttributeName, TileLogoSmall);
				list.Add(item);
			}
			if (!string.IsNullOrWhiteSpace(TileLogoWide))
			{
				FieldWriteInfo item2 = new FieldWriteInfo(manifestDefs.DefaultTilePath, manifestDefs.WideTileLogoAttributeName, TileLogoWide);
				list.Add(item2);
			}
			if (!string.IsNullOrWhiteSpace(TileLogoLarge))
			{
				FieldWriteInfo item3 = new FieldWriteInfo(manifestDefs.DefaultTilePath, manifestDefs.LargeTileLogoAttributeName, TileLogoLarge);
				list.Add(item3);
			}
			if (list.Count <= 0)
			{
				return;
			}
			if (!writer.HasElement(manifestDefs.DefaultTilePath))
			{
				writer.AddChildElement(manifestDefs.VisualElementsPath, manifestDefs.DefaultTileElementPrefix, manifestDefs.DefaultTileElementName, null, null);
			}
			foreach (FieldWriteInfo item4 in list)
			{
				WriteOneField(writer, item4);
			}
		}

		private void WriteOptionalInitialRotationPreference(XmlDocWriter writer, AppxManifestDefs manifestDefs)
		{
			if (initialScreenOrientations.Count == 0)
			{
				return;
			}
			writer.AddChildElement(manifestDefs.VisualElementsPath, manifestDefs.InitialRotationPreferenceElementPrefix, manifestDefs.InitialRotationPreferenceElementName, null, null);
			foreach (AppxScreenOrientationType initialScreenOrientation in initialScreenOrientations)
			{
				Dictionary<string, string> dictionary = new Dictionary<string, string>();
				dictionary[manifestDefs.RotationPreferenceAttributeName] = ScreenOrientationMap.GetAppxScreenOrientationName(initialScreenOrientation);
				writer.AddChildElement(manifestDefs.InitialRotationPreferencePath, manifestDefs.RotationElementPrefix, manifestDefs.RotationElementName, dictionary, null);
			}
		}

		private void WriteOptionalFieldsInSplashScreen(XmlDocWriter writer, AppxManifestDefs manifestDefs)
		{
			if (!string.IsNullOrWhiteSpace(SplashScreen))
			{
				if (!writer.HasElement(manifestDefs.SplashScreenPath))
				{
					writer.AddChildElement(manifestDefs.VisualElementsPath, manifestDefs.SplashScreenElementPrefix, manifestDefs.SplashScreenElementName, null, null);
				}
				FieldWriteInfo info = new FieldWriteInfo(manifestDefs.SplashScreenPath, manifestDefs.SplashScreenAttributeName, SplashScreen);
				WriteOneField(writer, info);
			}
		}

		private void WriteLanguages(XmlDocWriter writer, AppxManifestDefs manifestDefs)
		{
			if (languages.Count == 0)
			{
				throw new ConverterException("No language is set");
			}
			writer.RemoveAllChildElements(manifestDefs.ResourcesPath);
			foreach (string language in languages)
			{
				Dictionary<string, string> dictionary = new Dictionary<string, string>();
				dictionary[manifestDefs.LanguageAttribute] = language;
				writer.AddChildElement(manifestDefs.ResourcesPath, null, manifestDefs.ResourceElementName, dictionary, null);
			}
		}

		private void WriteShareTargetExtension(XmlDocWriter writer, AppxManifestDefs manifestDefs)
		{
			if (shareTargetDataFormats == null || shareTargetDataFormats.Count == 0)
			{
				return;
			}
			if (!writer.HasElement(manifestDefs.ExtensionsElementPath))
			{
				writer.AddChildElement(manifestDefs.ApplicationPath, null, manifestDefs.ExtensionsElementName, null, null);
			}
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			dictionary[manifestDefs.CategoryAttributeName] = manifestDefs.ShareTargetCategory;
			writer.AddChildElement(manifestDefs.ExtensionsElementPath, "uap", manifestDefs.ExtensionElementName, dictionary, null);
			writer.AddChildElement(manifestDefs.UapExtensionElementPath + "[@Category='windows.shareTarget']", "uap", manifestDefs.ShareTargetElementName, null, null);
			foreach (string shareTargetDataFormat in shareTargetDataFormats)
			{
				writer.AddChildElement(manifestDefs.ShareTargetElementPath, "uap", manifestDefs.DataFormatElementName, null, shareTargetDataFormat);
			}
		}

		[SuppressMessage("Microsoft.Globalization", "CA1308:NormalizeStringsToUpperCase", Justification = "Schema requires lowercase protocol names.")]
		private void WriteProtocolExtensions(XmlDocWriter writer, AppxManifestDefs manifestDefs)
		{
			if (protocolExtensions == null || protocolExtensions.Count == 0)
			{
				return;
			}
			if (!writer.HasElement(manifestDefs.ExtensionsElementPath))
			{
				writer.AddChildElement(manifestDefs.ApplicationPath, null, manifestDefs.ExtensionsElementName, null, null);
			}
			foreach (KeyValuePair<string, string> protocolExtension in protocolExtensions)
			{
				Dictionary<string, string> dictionary = new Dictionary<string, string>();
				dictionary[manifestDefs.CategoryAttributeName] = manifestDefs.ProtocolCategory;
				writer.AddChildElement(manifestDefs.ExtensionsElementPath, "uap", manifestDefs.ExtensionElementName, dictionary, null);
				dictionary = new Dictionary<string, string>();
				dictionary[manifestDefs.NameAttribute] = protocolExtension.Key.ToLowerInvariant();
				if (!string.IsNullOrWhiteSpace(protocolExtension.Value))
				{
					dictionary[manifestDefs.ReturnResultsAttributeName] = protocolExtension.Value;
				}
				writer.AddChildElement(manifestDefs.UapExtensionElementPath + "[@Category='windows.protocol']", "uap", manifestDefs.ProtocolElementName, dictionary, null);
			}
		}

		private void WriteBackgroundTaskExtensions(XmlDocWriter writer, AppxManifestDefs manifestDefs)
		{
			if (backgroundTaskExtensions == null || backgroundTaskExtensions.Count <= 0)
			{
				return;
			}
			if (!writer.HasElement(manifestDefs.ExtensionsElementPath))
			{
				writer.AddChildElement(manifestDefs.ApplicationPath, null, manifestDefs.ExtensionsElementName, null, null);
			}
			foreach (KeyValuePair<string, BackgroundTaskInfo> backgroundTaskExtension in backgroundTaskExtensions)
			{
				string key = backgroundTaskExtension.Key;
				HashSet<string> taskTypes = backgroundTaskExtension.Value.TaskTypes;
				Dictionary<string, string> dictionary = new Dictionary<string, string>();
				dictionary[manifestDefs.CategoryAttributeName] = manifestDefs.BackgroundTasksCategory;
				if (!string.IsNullOrEmpty(backgroundTaskExtension.Value.Executable))
				{
					dictionary[manifestDefs.ExecutableAttributeName] = backgroundTaskExtension.Value.Executable;
				}
				dictionary[manifestDefs.EntryPointAttributeName] = key;
				writer.AddChildElement(manifestDefs.ExtensionsElementPath, null, manifestDefs.ExtensionElementName, dictionary, null);
				string text = manifestDefs.ExtensionElementPath + string.Format(CultureInfo.InvariantCulture, "[@{0}='{1}' and @{2}='{3}']", manifestDefs.CategoryAttributeName, manifestDefs.BackgroundTasksCategory, manifestDefs.EntryPointAttributeName, key);
				writer.AddChildElement(text, null, manifestDefs.BackgroundTasksElementName, null, null);
				string parentPath = text + "/" + XmlUtilites.MakeElementPath(XmlConstants.XmlManifestDefaultPrefix, manifestDefs.BackgroundTasksElementName);
				foreach (string item in taskTypes)
				{
					Dictionary<string, string> dictionary2 = new Dictionary<string, string>();
					dictionary2[manifestDefs.TypeAttributeName] = item;
					writer.AddChildElement(parentPath, null, manifestDefs.TaskElementName, dictionary2, null);
				}
			}
		}

		private void WriteInProcessServerExtensions(XmlDocWriter writer, AppxManifestDefs manifestDefs)
		{
			if (inProcessServerExtensions == null || inProcessServerExtensions.Count <= 0)
			{
				return;
			}
			if (!writer.HasElement(manifestDefs.PackageExtensionsElementPath))
			{
				writer.AddChildElement(manifestDefs.PackagePath, null, manifestDefs.ExtensionsElementName, null, null);
			}
			foreach (KeyValuePair<string, List<InProcessServerClassInfo>> inProcessServerExtension in inProcessServerExtensions)
			{
				Dictionary<string, string> dictionary = new Dictionary<string, string>();
				dictionary[manifestDefs.CategoryAttributeName] = manifestDefs.InProcessServerCategory;
				writer.AddChildElement(manifestDefs.PackageExtensionsElementPath, null, manifestDefs.ExtensionElementName, dictionary, null);
				string text = manifestDefs.PackageExtensionElementPath + string.Format(CultureInfo.InvariantCulture, "[@{0}='{1}']", new object[2] { manifestDefs.CategoryAttributeName, manifestDefs.InProcessServerCategory });
				writer.AddChildElement(text, null, manifestDefs.InProcessServerElementName, null, null);
				string parentPath = text + "/" + XmlUtilites.MakeElementPath(XmlConstants.XmlManifestDefaultPrefix, manifestDefs.InProcessServerElementName);
				writer.AddChildElement(parentPath, null, manifestDefs.PathElementName, null, inProcessServerExtension.Key);
				foreach (InProcessServerClassInfo item in inProcessServerExtension.Value)
				{
					Dictionary<string, string> dictionary2 = new Dictionary<string, string>();
					dictionary2[manifestDefs.ActivatableClassIdAttributeName] = item.ClassId;
					dictionary2[manifestDefs.ThreadingModelAttributeName] = item.ThreadingModel;
					writer.AddChildElement(parentPath, null, manifestDefs.ActivatableClassElementName, dictionary2, null);
				}
			}
		}

		private void WriteCapabilities(XmlDocWriter writer, AppxManifestDefs manifestDefs)
		{
			writer.RemoveAllChildElements(manifestDefs.CapabilitiesPath);
			foreach (string restrictedCapability in restrictedCapabilities)
			{
				Dictionary<string, string> dictionary = new Dictionary<string, string>();
				dictionary[manifestDefs.CapabilityAttributeName] = restrictedCapability;
				writer.AddChildElement(manifestDefs.CapabilitiesPath, "rescap", manifestDefs.CapabilityElementName, dictionary, null);
			}
			foreach (string softwareCapability in softwareCapabilities)
			{
				Dictionary<string, string> dictionary2 = new Dictionary<string, string>();
				dictionary2[manifestDefs.CapabilityAttributeName] = softwareCapability;
				string prefix = null;
				if (IsUapCapability(softwareCapability))
				{
					prefix = "uap";
				}
				writer.AddChildElement(manifestDefs.CapabilitiesPath, prefix, manifestDefs.CapabilityElementName, dictionary2, null);
			}
			foreach (string deviceCapability in deviceCapabilities)
			{
				Dictionary<string, string> dictionary3 = new Dictionary<string, string>();
				dictionary3[manifestDefs.CapabilityAttributeName] = deviceCapability;
				writer.AddChildElement(manifestDefs.CapabilitiesPath, null, manifestDefs.DeviceCapabilityElementName, dictionary3, null);
			}
		}
	}
}