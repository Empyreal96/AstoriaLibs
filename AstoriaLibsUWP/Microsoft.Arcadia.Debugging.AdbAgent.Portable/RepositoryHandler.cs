using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Arcadia.Marketplace.PackageObjectModel;
using Microsoft.Arcadia.Marketplace.Utils.Portable;

namespace Microsoft.Arcadia.Debugging.AdbAgent.Portable
{
	public class RepositoryHandler : IPortableRepositoryHandler
	{
		private const string AppxProjectDirectory = "AppxProject";

		private const string ApkExtractDirectory = "ApkExtract";

		private const string ApkFileName = "payload.apk";

		private const string MakePriConfigFileName = "PriConfig.xml";

		private const string MakePriFolderName = "MakePri";

		private const string MakePriFileName = "MakePri.exe";

		private const string X86DirectoryName = "x86";

		private const string X64DirectoryName = "x64";

		private const string ARMDirectoryName = "ARM";

		private const int NTFSMaxDirectoryNameLength = 255;

		private string projectWorkingPath;

		private string apkExtractionPath;

		private string appxProjectPath;

		private string apkPath;

		private IAgentConfiguration configuration;

		private IFactory factory;

		private bool initialized;

		public RepositoryHandler(IFactory factory, IAgentConfiguration configuration)
		{
			if (factory == null)
			{
				throw new ArgumentNullException("factory");
			}
			if (configuration == null)
			{
				throw new ArgumentNullException("configuration");
			}
			this.factory = factory;
			this.configuration = configuration;
		}

		public async Task InitializeAsync(IPackageDetails apkDetails)
		{
			BuildsPaths();
			CleanUpDirectories();
			GenerateDirectories();
			initialized = true;
		}

		public string RetrievePackageFilePath()
		{
			if (!initialized)
			{
				throw new InvalidOperationException("Repository must be initialized.");
			}
			return apkPath;
		}

		public string RetrievePackageExtractionPath()
		{
			if (!initialized)
			{
				throw new InvalidOperationException("Repository must be initialized.");
			}
			return apkExtractionPath;
		}

		public string RetrieveMakePriToolPath()
		{
			if (!initialized)
			{
				throw new InvalidOperationException("Repository must be initialized.");
			}
			SystemArchitecture architecture = factory.CreateSystemInformation().Architecture;
			string text = Path.Combine(new string[2] { configuration.ToolsDirectory, "MakePri" });
			switch (architecture)
			{
				case SystemArchitecture.Arm:
					text = Path.Combine(new string[2] { text, "ARM" });
					break;
				case SystemArchitecture.X86:
				case SystemArchitecture.X64:
					text = Path.Combine(new string[2] { text, "x86" });
					break;
				default:
					throw new InvalidOperationException("Unsupported architecture");
			}
			return Path.Combine(new string[2] { text, "MakePri.exe" });
		}

		public string RetrieveMakePriConfigFilePath()
		{
			if (!initialized)
			{
				throw new InvalidOperationException("Repository must be initialized.");
			}
			return Path.Combine(new string[2] { projectWorkingPath, "PriConfig.xml" });
		}

		public string GetAppxEntryAppTemplatePath(AppxPackageConfiguration config)
		{
			if (!initialized)
			{
				throw new InvalidOperationException("Repository must be initialized.");
			}
			if (config == null)
			{
				throw new ArgumentNullException("config");
			}
			string text;
			if (config.PackageType == AppxPackageType.Phone)
			{
				text = "Phone";
			}
			else
			{
				if (config.PackageType != AppxPackageType.Tablet)
				{
					throw new InvalidOperationException("Unexpected package type " + config.PackageType);
				}
				text = "Tablet";
			}
			string text2;
			if (config.PackageArch == AppxPackageArchitecture.X86)
			{
				text2 = "x86";
			}
			else if (config.PackageArch == AppxPackageArchitecture.X64)
			{
				text2 = "x64";
			}
			else
			{
				if (config.PackageArch != 0)
				{
					throw new InvalidOperationException("Unexpected architecture " + config.PackageArch);
				}
				text2 = "ARM";
			}
			return Path.Combine(new string[3] { configuration.RootAppxTemplateDirectory, text, text2 });
		}

		public string GetAppxProjectRootFolder(AppxPackageConfiguration config)
		{
			if (!initialized)
			{
				throw new InvalidOperationException("Repository must be initialized.");
			}
			return appxProjectPath;
		}

		public string RetrieveAndroidAppPackageToolPath()
		{
			throw new NotImplementedException();
		}

		public void Clean()
		{
			if (!initialized)
			{
				throw new InvalidOperationException("Repository must be initialized.");
			}
			CleanUpDirectories();
		}

		private static string BuildSafeDirectoryName(string fileName)
		{
			string text = Path.GetFileNameWithoutExtension(fileName);
			if (text.Length > 255)
			{
				text = text.Substring(0, 255);
			}
			char[] invalidFileNameChars = Path.GetInvalidFileNameChars();
			char[] array = invalidFileNameChars;
			foreach (char c in array)
			{
				text = text.Replace(c.ToString(), "_");
			}
			return text;
		}

		private void BuildsPaths()
		{
			projectWorkingPath = GetWorkingDirectoryPath();
			appxProjectPath = Path.Combine(new string[2] { projectWorkingPath, "AppxProject" });
			apkPath = Path.Combine(new string[2] { projectWorkingPath, "payload.apk" });
			apkExtractionPath = Path.Combine(new string[2] { projectWorkingPath, "ApkExtract" });
		}

		private string GetWorkingDirectoryPath()
		{
			string text = Guid.NewGuid().ToString();
			return Path.Combine(new string[2] { configuration.AppxLayoutRoot, text });
		}

		private void GenerateDirectories()
		{
			PortableUtilsServiceLocator.FileUtils.CreateDirectory(projectWorkingPath);
			PortableUtilsServiceLocator.FileUtils.CreateDirectory(appxProjectPath);
			PortableUtilsServiceLocator.FileUtils.CreateDirectory(apkExtractionPath);
		}

		private void CleanUpDirectories()
		{
			if (PortableUtilsServiceLocator.FileUtils.DirectoryExists(projectWorkingPath))
			{
				PortableUtilsServiceLocator.FileUtils.DeleteDirectory(projectWorkingPath);
			}
		}
	}
}