using Microsoft.Arcadia.Marketplace.PackageObjectModel;

namespace Microsoft.Arcadia.Debugging.AdbAgent.Portable
{
	internal class PackageInformation : IPackageInformation
	{
		public string PackageIdentityName { get; set; }

		public string PackagePublisher { get; set; }

		public string PackagePublisherDisplayName { get; set; }
	}
}