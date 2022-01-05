using System.Diagnostics.CodeAnalysis;
using Microsoft.Arcadia.Marketplace.Utils.Portable;

namespace Microsoft.Arcadia.Marketplace.Converter
{
	internal class AppxManifestDefs
	{
		private const string IdentityElementName = "Identity";

		private const string PropertiesElementName = "Properties";

		private const string ApplicationsElementName = "Applications";

		private const string ApplicationElementName = "Application";

		private const string DisplayNameElementName = "DisplayName";

		private const string PublisherDisplayNameElementName = "PublisherDisplayName";

		private const string ResourcesElementName = "Resources";

		private const string CapabilitiesElementName = "Capabilities";

		public readonly string NameAttribute = "Name";

		public readonly string PackagePublisherAttribute = "Publisher";

		public readonly string VersionAttribute = "Version";

		public readonly string ProcessorArchitectureAttribute = "ProcessorArchitecture";

		public readonly string LanguageAttribute = "Language";

		public readonly string DisplayNameAttribute = "DisplayName";

		public readonly string MediumTileLogoAttribute = "Square150x150Logo";

		public readonly string WideTileLogoAttributeName = "Wide310x150Logo";

		public readonly string SplashScreenAttributeName = "Image";

		public readonly string CategoryAttributeName = "Category";

		public readonly string CapabilityAttributeName = "Name";

		public readonly string ApplicationIdAttribute = "Id";

		public readonly string RotationPreferenceAttributeName = "Preference";

		public readonly string BackgroundColorAttributeName = "BackgroundColor";

		public readonly string ReturnResultsAttributeName = "ReturnResults";

		public readonly string EntryPointAttributeName = "EntryPoint";

		public readonly string ExecutableAttributeName = "Executable";

		public readonly string TypeAttributeName = "Type";

		public readonly string ActivatableClassIdAttributeName = "ActivatableClassId";

		public readonly string ThreadingModelAttributeName = "ThreadingModel";

		public readonly string ResourceElementName = "Resource";

		public readonly string DefaultTileElementName = "DefaultTile";

		public readonly string SplashScreenElementName = "SplashScreen";

		public readonly string InitialRotationPreferenceElementName = "InitialRotationPreference";

		public readonly string RotationElementName = "Rotation";

		public readonly string ExtensionsElementName = "Extensions";

		public readonly string ExtensionElementName = "Extension";

		public readonly string ProjectAElementName = "AowApp";

		public readonly string PayloadNameElementName = "PayloadName";

		public readonly string PayloadVersionElementName = "PayloadVersion";

		public readonly string ShareTargetElementName = "ShareTarget";

		public readonly string ProtocolElementName = "Protocol";

		public readonly string DataFormatElementName = "DataFormat";

		public readonly string ShareTargetCategory = "windows.shareTarget";

		public readonly string ProtocolCategory = "windows.protocol";

		public readonly string FileTypeTypeAssociateCategory = "windows.fileTypeAssociation";

		public readonly string BackgroundTasksCategory = "windows.backgroundTasks";

		public readonly string InProcessServerCategory = "windows.activatableClass.inProcessServer";

		public readonly string FileTypeAssociationElementName = "FileTypeAssociation";

		public readonly string SupportedFileTypesElementName = "SupportedFileTypes";

		public readonly string FileTypeElementName = "FileType";

		public readonly string CapabilityElementName = "Capability";

		public readonly string DeviceCapabilityElementName = "DeviceCapability";

		public readonly string PackageElementName = "Package";

		public readonly string BackgroundTasksElementName = "BackgroundTasks";

		public readonly string TaskElementName = "Task";

		public readonly string InProcessServerElementName = "InProcessServer";

		public readonly string PathElementName = "Path";

		public readonly string ActivatableClassElementName = "ActivatableClass";

		public string PackagePath { get; private set; }

		public string IdentityPath { get; private set; }

		public string PropertiesDisplayNamePath { get; private set; }

		public string PropertiesPublisherDisplayNamePath { get; private set; }

		public string PropertiesLogoPath { get; private set; }

		public string ResourcesPath { get; private set; }

		public string CapabilitiesPath { get; private set; }

		public string ApplicationPath { get; private set; }

		public string ExtensionsElementPath { get; private set; }

		public string ExtensionElementPath { get; private set; }

		public string MobileExtensionElementPath { get; private set; }

		public string MobileProjectAExtensionElementPath { get; private set; }

		public string UapExtensionElementPath { get; private set; }

		public string ShareTargetElementPath { get; private set; }

		public string FileTypeAssociationElementPath { get; private set; }

		public string PackageExtensionsElementPath { get; private set; }

		public string PackageExtensionElementPath { get; private set; }

		[SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "StypeCop reports error on some abbreviations such as appx, xmlns")]
		public string VisualElementsPath { get; protected set; }

		[SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "StypeCop reports error on some abbreviations such as appx, xmlns")]
		public string DefaultTilePath { get; protected set; }

		public string DefaultTileElementPrefix { get; protected set; }

		public string SplashScreenElementPrefix { get; protected set; }

		public string InitialRotationPreferenceElementPrefix { get; protected set; }

		public string RotationElementPrefix { get; protected set; }

		[SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "StypeCop reports error on some abbreviations such as appx, xmlns")]
		public string SplashScreenPath { get; protected set; }

		[SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "StypeCop reports error on some abbreviations such as appx, xmlns")]
		public string InitialRotationPreferencePath { get; protected set; }

		public string AppLogoAttribute { get; protected set; }

		public string SmallTileLogoAttributeName { get; protected set; }

		public string LargeTileLogoAttributeName { get; protected set; }

		[SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "StypeCop reports error on some abbreviations such as appx")]
		public AppxManifestDefs(string prefixDefault, string prefixMobile, string prefixUap)
		{
			PackagePath = XmlUtilites.MakeElementPath(prefixDefault, PackageElementName);
			IdentityPath = XmlUtilites.MakeElementPath(prefixDefault, PackageElementName, "Identity");
			PropertiesDisplayNamePath = XmlUtilites.MakeElementPath(prefixDefault, PackageElementName, "Properties", "DisplayName");
			PropertiesPublisherDisplayNamePath = XmlUtilites.MakeElementPath(prefixDefault, PackageElementName, "Properties", "PublisherDisplayName");
			ResourcesPath = XmlUtilites.MakeElementPath(prefixDefault, PackageElementName, "Resources");
			CapabilitiesPath = XmlUtilites.MakeElementPath(prefixDefault, PackageElementName, "Capabilities");
			PropertiesLogoPath = XmlUtilites.MakeElementPath(prefixDefault, PackageElementName, "Properties", "Logo");
			ApplicationPath = XmlUtilites.MakeElementPath(prefixDefault, PackageElementName, "Applications", "Application");
			ExtensionsElementPath = XmlUtilites.MakeElementPath(prefixDefault, PackageElementName, "Applications", "Application", ExtensionsElementName);
			ExtensionElementPath = XmlUtilites.MakeElementPath(prefixDefault, PackageElementName, "Applications", "Application", ExtensionsElementName, ExtensionElementName);
			MobileExtensionElementPath = ExtensionsElementPath + "/" + XmlUtilites.MakeElementPath(prefixMobile, ExtensionElementName);
			MobileProjectAExtensionElementPath = MobileExtensionElementPath + "/" + XmlUtilites.MakeElementPath(prefixMobile, ProjectAElementName);
			UapExtensionElementPath = ExtensionsElementPath + "/" + XmlUtilites.MakeElementPath(prefixUap, ExtensionElementName);
			ShareTargetElementPath = UapExtensionElementPath + "/" + XmlUtilites.MakeElementPath(prefixUap, ShareTargetElementName);
			FileTypeAssociationElementPath = UapExtensionElementPath + "/" + XmlUtilites.MakeElementPath(prefixUap, FileTypeAssociationElementName);
			VisualElementsPath = ApplicationPath + "/" + XmlUtilites.MakeElementPath(prefixUap, "VisualElements");
			DefaultTilePath = ApplicationPath + "/" + XmlUtilites.MakeElementPath(prefixUap, "VisualElements", DefaultTileElementName);
			DefaultTileElementPrefix = prefixUap;
			SplashScreenPath = ApplicationPath + "/" + XmlUtilites.MakeElementPath(prefixUap, "VisualElements", SplashScreenElementName);
			SplashScreenElementPrefix = prefixUap;
			InitialRotationPreferencePath = ApplicationPath + "/" + XmlUtilites.MakeElementPath(prefixUap, "VisualElements", InitialRotationPreferenceElementName);
			InitialRotationPreferenceElementPrefix = prefixUap;
			RotationElementPrefix = prefixUap;
			AppLogoAttribute = "Square44x44Logo";
			SmallTileLogoAttributeName = "Square71x71Logo";
			LargeTileLogoAttributeName = "Square310x310Logo";
			PackageExtensionsElementPath = XmlUtilites.MakeElementPath(prefixDefault, PackageElementName, ExtensionsElementName);
			PackageExtensionElementPath = XmlUtilites.MakeElementPath(prefixDefault, PackageElementName, ExtensionsElementName, ExtensionElementName);
		}
	}
}