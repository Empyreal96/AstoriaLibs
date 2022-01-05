namespace Microsoft.Arcadia.Marketplace.PackageObjectModel.Apk
{
	public interface IDevReportReceiver : IDevReportIntentReceiver
	{
		string Permission { get; }
	}
}