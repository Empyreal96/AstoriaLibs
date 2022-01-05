using Microsoft.Arcadia.Marketplace.PackageObjectModel.Portable.Apk;

namespace Microsoft.Arcadia.Marketplace.Converter.Portable
{
	public class PackageObjectDefaults
	{
		public ManifestStringResource ApplicationNameResource { get; set; }

		public ManifestStringResource ApplicationIconResource { get; set; }

		public string ApplicationIconFilePath { get; set; }

		public uint ApplicationIconResourceId { get; set; }
	}
}