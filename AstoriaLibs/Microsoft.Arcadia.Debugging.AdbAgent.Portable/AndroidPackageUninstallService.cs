using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Arcadia.Marketplace.Utils.Log;
using Microsoft.Arcadia.Marketplace.Utils.Portable;

namespace Microsoft.Arcadia.Debugging.AdbAgent.Portable
{
	internal class AndroidPackageUninstallService
	{
		private IFactory factory;

		public AndroidPackageUninstallService(IFactory factory)
		{
			if (factory == null)
			{
				throw new ArgumentNullException("factory");
			}
			this.factory = factory;
		}

		public async Task<AndroidPackageUninstallResult> UninstallAndroidPackageAsync(string androidPackageName)
		{
			if (string.IsNullOrWhiteSpace(androidPackageName))
			{
				throw new ArgumentException("Must not be null or be whitespace.", "packageName");
			}
			AndroidPackageResolverService resolverService = new AndroidPackageResolverService(factory);
			AppxPackage uninstallPackage = resolverService.ResolveAppxFromAndroidPackage(androidPackageName);
			AndroidPackageUninstallResult returnResult;
			if (uninstallPackage != null)
			{
				IPackageManager packageManager = factory.CreatePackageManager();
				LoggerCore.Log("Uninstalling Package. Package Name = {0} with Package Publisher = {1}.", uninstallPackage.PackageName, uninstallPackage.PackagePublisher);
				DateTime beforeTime = DateTime.Now;
				AppUninstallResult uninstallResult = await packageManager.UninstallAppsAsync(uninstallPackage);
				TimeSpan afterTime = DateTime.Now.Subtract(beforeTime);
				LoggerCore.Log("Uninstall Package Result: {0}. Took: {1} second(s).", uninstallResult.ToString(), afterTime.TotalSeconds);
				if (uninstallResult == AppUninstallResult.Success)
				{
					returnResult = AndroidPackageUninstallResult.Success;
					RemovePackageDirectory(uninstallPackage.PackageLocation);
				}
				else
				{
					returnResult = AndroidPackageUninstallResult.UninstallError;
				}
			}
			else
			{
				returnResult = AndroidPackageUninstallResult.NotFound;
			}
			LoggerCore.Log("AndroidPackageUninstallService result: " + returnResult);
			return returnResult;
		}

		private static void RemovePackageDirectory(string packageLocation)
		{
			try
			{
				if (PortableUtilsServiceLocator.FileUtils.DirectoryExists(packageLocation))
				{
					PortableUtilsServiceLocator.FileUtils.DeleteDirectory(packageLocation);
				}
			}
			catch (Exception ex)
			{
				if (ex is IOException || ex is UnauthorizedAccessException)
				{
					LoggerCore.Log(ex);
					return;
				}
				throw;
			}
		}
	}
}
