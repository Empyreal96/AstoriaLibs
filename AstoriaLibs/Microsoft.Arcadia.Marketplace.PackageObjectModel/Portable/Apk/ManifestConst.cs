using System.Diagnostics.CodeAnalysis;

namespace Microsoft.Arcadia.Marketplace.PackageObjectModel.Portable.Apk
{
	public static class ManifestConst
	{
		public const string ManifestFileName = "AndroidManifest.xml";

		public const string ManifestAndroidNamespace = "http://schemas.android.com/apk/res/android";

		public const string ManifestAmazonNamespace = "http://schemas.amazon.com/apk/res/android";

		public const string ManifestRootElementName = "manifest";

		public const string ManifestPackageAttributeName = "package";

		public const string ManifestApplicationElementName = "application";

		public const string ManifestApplicationPermissionAttributeName = "permission";

		public const string ManifestUsesPermissionElementName = "uses-permission";

		public const string ManifestLabelAttributeName = "label";

		public const string ManifestIconAttributeName = "icon";

		public const string ManifestVersionCodeAttributeName = "versionCode";

		public const string ManifestVersionNameAttributeName = "versionName";

		public const string ManifestActivityElementName = "activity";

		public const string ManifestActivityAliasElementName = "activity-alias";

		public const string ManifestActivityScreenOrientationAttributeName = "screenOrientation";

		public const string ManifestMetadataElementName = "meta-data";

		public const string ManifestActivityNameAttributeName = "name";

		public const string ManifestActivityLabelAttributeName = "label";

		public const string ManifestIActivityLaunchModeAttributeName = "launchMode";

		public const string ManifestActivityThemeAttributeName = "theme";

		public const string ManifestActivityAliasTargetAttributeName = "targetActivity";

		public const string ManifestIntentFilterElementName = "intent-filter";

		public const string ManifestActionElementName = "action";

		public const string ManifestActionNameAttributeName = "name";

		public const string ManifestCategoryElementName = "category";

		public const string ManifestCategoryNameAttributeName = "name";

		public const string ManifestDataElementName = "data";

		public const string ManifestDataMimeTypeAttributeName = "mimeType";

		public const string ManifestDataHostAttributeName = "host";

		public const string ManifestDataPortAttributeName = "port";

		public const string ManifestDataPathAttributeName = "path";

		public const string ManifestDataSchemeAttributeName = "scheme";

		public const string ManifestDataPathPrefixAttributeName = "pathPrefix";

		public const string ManifestDataPathPatternAttributeName = "pathPattern";

		public const string ManifestUsesPermissionNameAttribute = "name";

		public const string ManifestUsesPermissionSdkAttribute = "maxSdkVersion";

		public const string ManifestMetadataResourceAttribute = "resource";

		public const string ManifestMetadataNameAttribute = "name";

		public const string ManifestMetadataValueAttribute = "value";

		public const string ManifestUsesFeatureElementName = "uses-feature";

		public const string ManifestUsesFeatureNameAttribute = "name";

		public const string ManifestUsesFeatureRequiredAttribute = "required";

		public const string ManifestUsesFeatureGlesVersionAttribute = "glEsVersion";

		public const string ManifestContentProviderName = "provider";

		public const string ManifestContentProviderNameAttribute = "name";

		public const string ManifestContentProviderEnabledAttribute = "enabled";

		public const string ManifestUsesLibraryElementName = "uses-library";

		public const string ManifestUsesConfigurationElementName = "uses-configuration";

		public const string ManifestUsesSdkElementName = "uses-sdk";

		public const string ManifestSupportsScreensElementName = "supports-screens";

		public const string ManifestSupportsGLTextureElementName = "supports-gl-texture";

		public const string ManifestCompatibleScreensElementName = "compatible-screens";

		public const string ManifestReceiverElementName = "receiver";

		public const string ManifestReceiverNameAttributeName = "name";

		public const string ManifestReceiverEnabledAttribute = "enabled";

		public const string ManifestReceiverPermissionAttribute = "permission";

		public const string ManifestMetadataBackgroundColorAttributeName = "windows-background";

		public const string ManifestUsesSdkminSdkVersionAttribute = "minSdkVersion";

		public const string ManifestUsesSdktargetSdkVersionAttribute = "targetSdkVersion";

		public const string ManifestUsesSdkmaxSdkVersionAttribute = "maxSdkVersion";

		[SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "GMS", Justification = "GMS is an acronym for Google Mobile Services.")]
		public const string ManifestGMSVersionAttributeValue = "com.google.android.gms.version";

		public const string ManifestPackageIdentityMetadataName = "com.microsoft.windows.package.identity.name";

		public const string ManifestPackagePublisherName = "com.microsoft.windows.package.identity.publisher";

		public const string ManifestPackagePublisherDisplayName = "com.microsoft.windows.package.properties.publisherdisplayname";

		public const string ManifestServiceElementName = "service";

		public const string ManifestServiceNameAttribute = "name";

		public const string ManifestServiceExportedAttribute = "exported";

		public const string ManifestServicePermissionAttributeName = "permission";

		public const string ManifestLauncherActivityName = "android.intent.category.LAUNCHER";

		public const string ManifestHomeCategoryName = "android.intent.category.HOME";

		public const string ManifestIntentActionMainName = "android.intent.action.MAIN";

		public const string ManifestIntentActionViewName = "android.intent.action.VIEW";

		public const string ManifestTestApplicationName = "Test Application";

		public const string ManifestTestApplicationIconResourceName = "@res:19901024";

		public const uint ManifestTestApplicationIconResourceId = 428871716u;
	}
}