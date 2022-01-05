using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Arcadia.Marketplace.PackageObjectModel;
using Microsoft.Arcadia.Marketplace.Utils.Log;
using Microsoft.Arcadia.Marketplace.Utils.Portable;

namespace Microsoft.Arcadia.Debugging.AdbAgent.Portable
{
	public class AppxDeployService
	{
		public IFactory Factory { get; private set; }

		public AppxDeployService(IFactory factory)
		{
			if (factory == null)
			{
				throw new ArgumentNullException("factory");
			}
			Factory = factory;
		}

		public async Task<PackageDeploymentResult> DeployAppxProjectAsync(IPortableRepositoryHandler repository)
		{
			if (repository == null)
			{
				throw new ArgumentNullException("repository");
			}
			string finalAppxPath = repository.GetAppxProjectRootFolder(null);
			if (!PortableUtilsServiceLocator.FileUtils.DirectoryExists(finalAppxPath))
			{
				throw new InvalidOperationException("The APPX directory does not exist.");
			}
			IPackageManager packageManager = Factory.CreatePackageManager();
			Uri manifestUri = new Uri(Path.Combine(new string[2] { finalAppxPath, "AppxManifest.xml" }));
			LoggerCore.Log("Installing APPX from Layout...");
			DateTime beforeTime = DateTime.Now;
			PackageDeploymentResult installResult = await packageManager.InstallAppFromFolderLayoutAsync(manifestUri, null);
			LoggerCore.Log("Installation took: " + DateTime.Now.Subtract(beforeTime).TotalSeconds + " second(s).");
			if (installResult.Error == null)
			{
				LoggerCore.Log("Installation result: Success");
				CleanUnnecessaryDirectories(repository);
			}
			else
			{
				LoggerCore.Log("Installation error code: {0}", installResult.Error);
				LoggerCore.Log("Installation extended error code: {0}", installResult.ExtendedError);
				CleanAllDirectories(repository);
			}
			return installResult;
		}

		[SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "Microsoft.Arcadia.Marketplace.Utils.Log.LoggerCore.Log(Microsoft.Arcadia.Marketplace.Utils.Log.LoggerCore+LogLevels,System.String)", Justification = "Log Statement.")]
		private static void CleanUnnecessaryDirectories(IPortableRepositoryHandler repository)
		{
			try
			{
				DeleteApkFile(repository);
				DeleteApkExtractionPath(repository);
				DeleteMakePriConfigFile(repository);
			}
			catch (Exception exp)
			{
				if (ExceptionUtils.IsIOException(exp))
				{
					LoggerCore.Log(LoggerCore.LogLevels.Error, "Error removing unnecessary files from APPX package location following successful installation.");
					LoggerCore.Log(exp);
					return;
				}
				throw;
			}
		}

		private static void DeleteApkExtractionPath(IPortableRepositoryHandler repository)
		{
			IPortableFileUtils fileUtils = PortableUtilsServiceLocator.FileUtils;
			string text = repository.RetrievePackageExtractionPath();
			if (fileUtils.DirectoryExists(text))
			{
				PortableUtilsServiceLocator.FileUtils.DeleteDirectory(text);
			}
		}

		private static void DeleteApkFile(IPortableRepositoryHandler repository)
		{
			IPortableFileUtils fileUtils = PortableUtilsServiceLocator.FileUtils;
			string filePath = repository.RetrievePackageFilePath();
			if (fileUtils.FileExists(filePath))
			{
				fileUtils.DeleteFile(filePath);
			}
		}

		private static void DeleteMakePriConfigFile(IPortableRepositoryHandler repository)
		{
			IPortableFileUtils fileUtils = PortableUtilsServiceLocator.FileUtils;
			string filePath = repository.RetrieveMakePriConfigFilePath();
			if (fileUtils.FileExists(filePath))
			{
				fileUtils.DeleteFile(filePath);
			}
		}

		[SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "Microsoft.Arcadia.Marketplace.Utils.Log.LoggerCore.Log(Microsoft.Arcadia.Marketplace.Utils.Log.LoggerCore+LogLevels,System.String)", Justification = "Log Statement.")]
		private static void CleanAllDirectories(IPortableRepositoryHandler repository)
		{
			try
			{
				repository.Clean();
			}
			catch (Exception exp)
			{
				if (ExceptionUtils.IsIOException(exp))
				{
					LoggerCore.Log(LoggerCore.LogLevels.Error, "Error deleting APPX package location following unsuccessful installation.");
					LoggerCore.Log(exp);
					return;
				}
				throw;
			}
		}
	}
}