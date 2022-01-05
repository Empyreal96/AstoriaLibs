using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using Microsoft.Arcadia.Marketplace.Utils.Log;
using Microsoft.Arcadia.Marketplace.Utils.Portable;

namespace Microsoft.Arcadia.Debugging.AdbAgent.Portable
{
	public class AppxFileSystemCleanUpService
	{
		private IPortableFileUtils fileUtils;

		private IPackageManager packageManager;

		private IAgentConfiguration agentConfig;

		private HashSet<string> allPackagesLocations;

		private IList<string> packageLocationsToDelete;

		private bool ShouldCleanup => PortableUtilsServiceLocator.FileUtils.DirectoryExists(agentConfig.AppxLayoutRoot);

		public AppxFileSystemCleanUpService(IFactory factory)
		{
			if (factory == null)
			{
				throw new ArgumentNullException("factory");
			}
			packageManager = factory.CreatePackageManager();
			fileUtils = factory.CreatePortableFileUtils();
			agentConfig = factory.AgentConfiguration;
			packageLocationsToDelete = new List<string>();
			allPackagesLocations = new HashSet<string>();
		}

		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "No need to handle errors on clean up.")]
		[SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "Microsoft.Arcadia.Marketplace.Utils.Log.LoggerCore.Log(Microsoft.Arcadia.Marketplace.Utils.Log.LoggerCore+LogLevels,System.String)", Justification = "Log Statement.")]
		[SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "Microsoft.Arcadia.Marketplace.Utils.Log.LoggerCore.Log(System.String)", Justification = "Log Statement.")]
		public void RemoveStaleAppxLayout()
		{
			try
			{
				if (!ShouldCleanup)
				{
					LoggerCore.Log("Skipped packages cleanup as the APPX target directory doesn't exist.");
					return;
				}
				LoggerCore.Log("Starting deletion of stale packages...");
				BuildAllPackageLocations();
				BuildStaleAppxPackageList();
				DeleteStalePackages();
				LoggerCore.Log("Finished deletion of stale packages.");
			}
			catch (Exception exp)
			{
				LoggerCore.Log(LoggerCore.LogLevels.Error, exp);
			}
		}

		private void BuildAllPackageLocations()
		{
			IList<AppxPackage> list = packageManager.FindPackages();
			LoggerCore.Log("Package Manager returned {0} installed package(s).", list.Count);
			foreach (AppxPackage item in list)
			{
				if (item.PackageLocation != null)
				{
					allPackagesLocations.Add(Path.GetDirectoryName(item.PackageLocation.ToLower()));
				}
			}
		}

		private void BuildStaleAppxPackageList()
		{
			string[] directories = fileUtils.GetDirectories(agentConfig.AppxLayoutRoot);
			string[] array = directories;
			foreach (string text in array)
			{
				if (IsPackageLocationStale(text))
				{
					packageLocationsToDelete.Add(text.ToLower());
				}
			}
			LoggerCore.Log("Found {0} stale package(s).", packageLocationsToDelete.Count);
		}

		private bool IsPackageLocationStale(string packageFolder)
		{
			return !allPackagesLocations.Contains(packageFolder.ToLower());
		}

		private void DeleteStalePackages()
		{
			foreach (string item in packageLocationsToDelete)
			{
				IOUtils.RemoveDirectory(item);
			}
		}
	}
}