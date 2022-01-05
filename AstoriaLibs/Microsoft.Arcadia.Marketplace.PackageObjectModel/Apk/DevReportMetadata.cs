namespace Microsoft.Arcadia.Marketplace.PackageObjectModel.Apk
{
	public class DevReportMetadata
	{
		public string Name { get; private set; }

		public DevReportMetadata(string name)
		{
			Name = name;
		}
	}
}