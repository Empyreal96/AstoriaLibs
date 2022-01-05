namespace Microsoft.Arcadia.Marketplace.PackageObjectModel.Apk
{
	public interface IDevReportObjectModel
	{
		IDevReportManifestObjectModel Manifest { get; }

		IDevReportDexModel Dex { get; }

		IDevReportMiddlewareModel Middleware { get; }

		IDevReportServicesModel Services { get; }

		IDevApkPackageModel Package { get; }
	}
}