namespace Microsoft.Arcadia.Marketplace.PackageObjectModel
{
	public interface IPackageInformation
	{
		string PackageIdentityName { get; set; }

		string PackagePublisher { get; set; }

		string PackagePublisherDisplayName { get; set; }
	}
}