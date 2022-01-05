using System.Runtime.Serialization;

namespace Microsoft.Arcadia.Marketplace.PackageObjectModel
{
	[DataContract]
	public sealed class AppxPackageConfiguration
	{
		[DataMember]
		public AppxPackageType PackageType { get; private set; }

		[DataMember]
		public AppxPackageArchitecture PackageArch { get; private set; }

		public AppxPackageConfiguration(AppxPackageType packageType, AppxPackageArchitecture packageArch)
		{
			PackageType = packageType;
			PackageArch = packageArch;
		}
	}
}