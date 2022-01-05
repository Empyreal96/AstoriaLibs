using System.Diagnostics.CodeAnalysis;

namespace Microsoft.Arcadia.Marketplace.PackageObjectModel.Portable
{
	public enum WorkerLogKey
	{
		None,
		[LogAnnotation("Erde0001", ReportSectionH1.InternalAppErrors)]
		DecoderErrorInternal,
		[LogAnnotation("Erde0002", ReportSectionH1.InternalAppErrors)]
		DecoderErrorInvalidManifestXml,
		[LogAnnotation("Erde0003", ReportSectionH1.InternalAppErrors)]
		DecoderErrorInvalidResourceFile,
		[LogAnnotation("Erde0004", ReportSectionH1.InternalAppErrors)]
		DecoderErrorManifestNoDirectApplicationChild,
		[LogAnnotation("Erde0005", ReportSectionH1.InternalAppErrors)]
		DecoderErrorConfigFileInvalid,
		[LogAnnotation("Erde0006", ReportSectionH1.InternalAppErrors)]
		DecoderErrorManifestNoVersionCode,
		[LogAnnotation("Erde0007", ReportSectionH1.InternalAppErrors)]
		DecoderErrorManifestStringOnlyField,
		[LogAnnotation("Inan0002", ReportSectionH1.AndroidComponents, Hidden = true)]
		AnalyserCustomPermissionLicensing,
		[LogAnnotation("Inan000F", ReportSectionH1.AndroidComponents)]
		AnalyserBackgroundColorNotSpecified,
		[LogAnnotation("Inan0010", ReportSectionH1.GMSDependencies, Hidden = true)]
		AnalyserAppIndexing,
		[LogAnnotation("Inan0011", ReportSectionH1.GMSDependencies, Hidden = true)]
		AnalyserSecurityProviderInstaller,
		[LogAnnotation("Inan0013", ReportSectionH1.AndroidComponents, Hidden = true)]
		AnalyserForegroundService,
		[LogAnnotation("Inan0029", ReportSectionH1.AndroidComponents, Hidden = true)]
		AnalyserManifestFeatureBluetooth,
		[LogAnnotation("Inan0033", ReportSectionH1.GMSDependencies, Hidden = true)]
		AnalyserPhotosphere,
		[LogAnnotation("Waan0031", ReportSectionH1.AndroidComponents)]
		AnalyserIconNotFound,
		[LogAnnotation("Waan0002", ReportSectionH1.AndroidComponents)]
		AnalyserIconUnsupportedQualifiers,
		[LogAnnotation("Waan0033", ReportSectionH1.GMSDependencies)]
		AnalyserCustomPermissionBillingV2,
		[LogAnnotation("Waan003B", ReportSectionH1.AndroidComponents, Hidden = true)]
		AnalyserManifestUnsupportedHardwareFeature,
		[LogAnnotation("Waan003F", ReportSectionH1.AndroidComponents, Hidden = true)]
		AnalyserBlocklistedContentProvider,
		[LogAnnotation("Waan0040", ReportSectionH1.AndroidComponents, Hidden = true)]
		AnalyserContentProviderNameMissing,
		[LogAnnotation("Waan0041", ReportSectionH1.AndroidComponents)]
		AnalyserAllScalesForCertainTypeMissing,
		[LogAnnotation("Waan0042", ReportSectionH1.AndroidComponents)]
		AnalyserManifestInvalidSchema,
		[LogAnnotation("Waan004B", ReportSectionH1.AndroidComponents, Hidden = true)]
		AnalyserAnalyticsMultipleTrackerIds,
		[LogAnnotation("Waan004C", ReportSectionH1.AndroidComponents, Hidden = true)]
		AnalyserAnalyticsCustomDimensionsOrMetrics,
		[LogAnnotation("Waan004D", ReportSectionH1.GMSDependencies, Hidden = true)]
		AnalyserMediationAdapters,
		[LogAnnotation("Waan004E", ReportSectionH1.GMSDependencies, Hidden = true)]
		AnalyserInterstitialAds,
		[LogAnnotation("Waan0046", ReportSectionH1.AndroidComponents, Hidden = true)]
		AnalyserManifestFeatureBluetoothLE,
		[LogAnnotation("Waan0047", ReportSectionH1.AndroidComponents, Hidden = true)]
		[SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "USB", Justification = "USB is an acronym for Universal Serial Bus.")]
		AnalyserManifestFeatureUSBAccessory,
		[SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "USB", Justification = "USB is an acronym for Universal Serial Bus.")]
		[LogAnnotation("Waan0048", ReportSectionH1.AndroidComponents, Hidden = true)]
		AnalyserManifestFeatureUSBHost,
		[LogAnnotation("Waan0049", ReportSectionH1.AndroidComponents, Hidden = true)]
		AnalyserGetMapsNonAsync,
		[LogAnnotation("Eran0002", ReportSectionH1.AndroidComponents)]
		AnalyserIconAllUnsupportedQualifiers,
		[LogAnnotation("Eran0051", ReportSectionH1.InternalAppErrors)]
		AnalyserErrorInternal,
		[LogAnnotation("Eran0052", ReportSectionH1.InternalAppErrors)]
		AnalyserErrorInternalNoManifest,
		[LogAnnotation("Eran0055", ReportSectionH1.InternalAppErrors)]
		AnalyserErrorManifestRootNotManifest,
		[LogAnnotation("Eran0056", ReportSectionH1.InternalAppErrors)]
		AnalyserErrorManifestMultipleApplicationChildren,
		[LogAnnotation("Eran005B", ReportSectionH1.AndroidComponents)]
		AnalyserErrorStockImages,
		[LogAnnotation("Eran005C", ReportSectionH1.AndroidComponents)]
		AnalyserErrorIconNotCorrectSize,
		[LogAnnotation("Eran005D", ReportSectionH1.AndroidComponents)]
		AnalyserErrorBackgroundColorInvalid,
		[LogAnnotation("Waan0062", ReportSectionH1.AndroidComponents, Hidden = true)]
		AnalyserUnsupportedDeeplinkFound,
		[LogAnnotation("Waan0064", ReportSectionH1.AndroidComponents, Hidden = true)]
		AnalyserAndroidActionBar,
		[LogAnnotation("Waan0065", ReportSectionH1.AndroidComponents, Hidden = true)]
		AnalyserAndroidCreateOptionsMenu,
		[LogAnnotation("Erco0001", ReportSectionH1.InternalAppErrors)]
		ConverterErrorInternal,
		[LogAnnotation("Erco0002", ReportSectionH1.InternalAppErrors)]
		PostConverterErrorInternal,
		[LogAnnotation("Erco0003", ReportSectionH1.InternalAppErrors)]
		ConverterIconPreviewErrorInternal,
		[LogAnnotation("Erwa0001", ReportSectionH1.InternalAppErrors, Hidden = true)]
		WebApiInternalServerError,
		[LogAnnotation("Erot0001", ReportSectionH1.InternalAppErrors)]
		ApkBlobDownloadError,
		[LogAnnotation("Erot0002", ReportSectionH1.InternalAppErrors)]
		ApkNoCertificateFoundError,
		[LogAnnotation("Waot0001", ReportSectionH1.InternalAppErrors)]
		ApkMultipleSignatureWarning
	}
}