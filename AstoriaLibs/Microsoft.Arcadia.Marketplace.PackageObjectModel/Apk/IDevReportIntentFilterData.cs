namespace Microsoft.Arcadia.Marketplace.PackageObjectModel.Apk
{
	public interface IDevReportIntentFilterData
	{
		string Scheme { get; }

		string Host { get; }

		string Port { get; }

		string Path { get; }

		string PathPattern { get; }

		string PathPrefix { get; }

		string MimeType { get; }
	}
}