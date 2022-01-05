namespace Microsoft.Arcadia.Marketplace.PackageObjectModel.Apk
{
	public class ManifestIntentFilterData : IDevReportIntentFilterData
	{
		public string Host { get; private set; }

		public string Port { get; private set; }

		public string Path { get; private set; }

		public string Scheme { get; private set; }

		public string MimeType { get; private set; }

		public string PathPattern { get; private set; }

		public string PathPrefix { get; private set; }

		public ManifestIntentFilterData(string host, string port, string path, string scheme, string mimeType, string pathPattern, string pathPrefix)
		{
			Host = host;
			Port = port;
			Path = path;
			Scheme = scheme;
			MimeType = mimeType;
			PathPattern = pathPattern;
			PathPrefix = pathPrefix;
		}
	}
}