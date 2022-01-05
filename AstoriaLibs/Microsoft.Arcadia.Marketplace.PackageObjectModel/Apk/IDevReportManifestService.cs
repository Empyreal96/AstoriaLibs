namespace Microsoft.Arcadia.Marketplace.PackageObjectModel.Apk
{
	public interface IDevReportManifestService : IDevReportIntentReceiver
	{
		string ServiceName { get; }

		string Permission { get; }
	}
}