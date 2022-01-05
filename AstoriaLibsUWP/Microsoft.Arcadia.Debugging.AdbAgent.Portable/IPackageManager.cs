using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Microsoft.Arcadia.Debugging.AdbAgent.Portable
{
	[ComVisible(false)]
	public interface IPackageManager
	{
		IList<AppxPackage> FindPackages();

		Task<PackageDeploymentResult> InstallAppFromFolderLayoutAsync(Uri manifestUri, IEnumerable<Uri> dependencyPackageUris);

		Task<AppUninstallResult> UninstallAppsAsync(AppxPackage package);
	}
}