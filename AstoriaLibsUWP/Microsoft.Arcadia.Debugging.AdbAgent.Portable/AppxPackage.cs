using System;

namespace Microsoft.Arcadia.Debugging.AdbAgent.Portable
{
	public class AppxPackage
	{
		public string PackageName { get; private set; }

		public string PackageFullName { get; private set; }

		public string PackagePublisher { get; private set; }

		public string PackageLocation { get; private set; }

		public AppxPackage(string packageName, string packageFullName, string packagePublisher, string packageLocation)
		{
			if (packageName == null)
			{
				throw new ArgumentNullException("packageName");
			}
			if (packageFullName == null)
			{
				throw new ArgumentNullException("packageFullName");
			}
			if (packagePublisher == null)
			{
				throw new ArgumentNullException("packagePublisher");
			}
			PackageName = packageName;
			PackageFullName = packageFullName;
			PackagePublisher = packagePublisher;
			PackageLocation = packageLocation;
		}
	}
}