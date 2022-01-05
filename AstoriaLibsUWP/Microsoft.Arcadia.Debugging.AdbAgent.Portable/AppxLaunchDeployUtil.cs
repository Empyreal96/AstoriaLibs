using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using Microsoft.Arcadia.Marketplace.PackageObjectModel;
using Microsoft.Arcadia.Marketplace.Utils.Log;
using Microsoft.Arcadia.Marketplace.Utils.Portable;

namespace Microsoft.Arcadia.Debugging.AdbAgent.Portable
{
	public static class AppxLaunchDeployUtil
	{
		public static async Task<string> InstallAppx(IFactory factory, string appxFilePath)
		{
			if (factory == null)
			{
				throw new ArgumentNullException("factory");
			}
			if (appxFilePath == null)
			{
				throw new ArgumentNullException("appxFilePath");
			}
			LoggerCore.Log("Installing APPX at " + appxFilePath);
			IPortableRepositoryHandler repository = factory.CreateRepository();
			await repository.InitializeAsync(new ApkDetails(Path.GetFileName(appxFilePath)));
			AppxPackageConfiguration packageConfig = new AppxPackageConfiguration(AppxPackageType.Phone, AppxPackageArchitecture.Arm);
			string projectRoot = repository.GetAppxProjectRootFolder(packageConfig);
			LoggerCore.Log("Extracting APPX to " + projectRoot);
			PortableZipUtils.ExtractAllFromZip(appxFilePath, projectRoot);
			AppxDeployService deployer = new AppxDeployService(factory);
			AppxPackage package = GetPackage(Path.Combine(new string[2] { projectRoot, "AppxManifest.xml" }));
			if (package != null)
			{
				LoggerCore.Log("Attempting to uninstall " + package.PackageName);
				AppUninstallResult result = await factory.CreatePackageManager().UninstallAppsAsync(package);
				LoggerCore.Log("Unistall result:" + result);
			}
			LoggerCore.Log("Deploying APPX");
			PackageDeploymentResult installResult = await deployer.DeployAppxProjectAsync(repository);
			string installResultText = AdbMessageStrings.FromPackageManagerInstallResult(installResult);
			if (installResult.Error == null)
			{
				LoggerCore.Log("Success: " + installResult);
				return installResultText;
			}
			LoggerCore.Log("Error: " + installResult);
			return installResultText;
		}

		[SuppressMessage("Microsoft.Design", "CA1054:UriParametersShouldNotBeStrings", MessageId = "1#", Justification = "User input")]
		public static void LaunchUri(IFactory factory, string uri)
		{
			if (factory == null)
			{
				throw new ArgumentNullException("factory");
			}
			if (uri == null)
			{
				throw new ArgumentNullException("uri");
			}
			factory.CreateUriLauncher().LaunchUri(new Uri(uri));
		}

		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Explicitly want to handle all exceptions")]
		private static AppxPackage GetPackage(string manifestPath)
		{
			LoggerCore.Log("Parsing manifest at " + manifestPath);
			try
			{
				string text = null;
				string text2 = null;
				XNamespace xNamespace = "http://schemas.microsoft.com/appx/2010/manifest";
				XDocument xDocument = XDocument.Load(manifestPath);
				LoggerCore.Log("Finding application node");
				XElement xElement = xDocument.Descendants(xNamespace + "Identity").FirstOrDefault();
				if (xElement == null)
				{
					LoggerCore.Log("Identity node not present in the manifest");
					return null;
				}
				using (IEnumerator<XAttribute> enumerator = xElement.Attributes("Name").GetEnumerator())
				{
					if (enumerator.MoveNext())
					{
						XAttribute current = enumerator.Current;
						text = current.Value;
						LoggerCore.Log("Found identity name: " + text);
					}
				}
				using (IEnumerator<XAttribute> enumerator2 = xElement.Attributes("Publisher").GetEnumerator())
				{
					if (enumerator2.MoveNext())
					{
						XAttribute current2 = enumerator2.Current;
						text2 = current2.Value;
						LoggerCore.Log("Found publisher: " + text2);
					}
				}
				if (text != null && text2 != null)
				{
					return new AppxPackage(text, string.Empty, text2, Path.GetDirectoryName(manifestPath));
				}
			}
			catch (Exception exp)
			{
				LoggerCore.Log(exp);
			}
			return null;
		}
	}
}