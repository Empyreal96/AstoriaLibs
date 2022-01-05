using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Arcadia.Debugging.AdbEngine.Portable;
using Microsoft.Arcadia.Debugging.AdbProtocol.Portable;
using Microsoft.Arcadia.Debugging.SharedUtils.Portable;
using Microsoft.Arcadia.Marketplace.Converter;
using Microsoft.Arcadia.Marketplace.Converter.Portable;
using Microsoft.Arcadia.Marketplace.Decoder.Portable;
using Microsoft.Arcadia.Marketplace.PackageObjectModel;
using Microsoft.Arcadia.Marketplace.PackageObjectModel.Apk;
using Microsoft.Arcadia.Marketplace.PackageObjectModel.Portable;
using Microsoft.Arcadia.Marketplace.PackageObjectModel.Portable.Apk;
using Microsoft.Arcadia.Marketplace.PackageObjectModel.Portable.Appx;
using Microsoft.Arcadia.Marketplace.Utils.Log;
using Microsoft.Arcadia.Marketplace.Utils.Portable;

namespace Microsoft.Arcadia.Debugging.AdbAgent.Portable
{
	internal class ApkInstallJob : ShellChannelJob
	{
		private enum PullAPkResult
		{
			Success,
			Error_FileNotFound,
			Errors_Others
		}

		private const string TestApplicationIconFileName = "msftwbfa-apktestapp.png";

		private const string TestApplicationIconAssemblyResourceName = "Microsoft.Arcadia.Debugging.AdbAgent.Portable.apktestapp.png";

		private ShellPmInstallParam param;

		private IFactory factory;

		private IPortableFileUtils fileUtils = PortableUtilsServiceLocator.FileUtils;

		private AppxPackageType appxPackageType;

		private ApkObjectModel apkModel;

		private string correlationId;

		public ApkInstallJob(ShellPmInstallParam param, IFactory factory, AppxPackageType appxPackageType)
		{
			if (param == null)
			{
				throw new ArgumentNullException("param");
			}
			if (factory == null)
			{
				throw new ArgumentNullException("factory");
			}
			this.param = param;
			this.factory = factory;
			this.appxPackageType = appxPackageType;
			base.IsWithinInteractiveShell = param.FromInteractiveShell;
		}

		protected override async Task<string> OnExecuteShellCommand()
		{
			return await InstallAppAsync();
		}

		private static string FormatAppInstallOutput(string apkFilePath, string message)
		{
			return string.Format(CultureInfo.InvariantCulture, "\tpkg: {0}\r\n{1}\r\n", new object[2]
			{
			string.IsNullOrEmpty(apkFilePath) ? "null" : apkFilePath,
			message
			});
		}


		private AppxPackageConfiguration CreateAppxConfiguration(SystemArchitecture architecture)
		{
            switch (architecture)
            {
				case SystemArchitecture.Arm:
					return new AppxPackageConfiguration(appxPackageType, AppxPackageArchitecture.Arm);
				case SystemArchitecture.X86:
					return new AppxPackageConfiguration(appxPackageType, AppxPackageArchitecture.X86);
				case SystemArchitecture.X64:
					return new AppxPackageConfiguration(appxPackageType, AppxPackageArchitecture.X64);
				default:
					throw new InvalidOperationException("The supplied CPU architecture is not supported in an APPX container");

			}
		}

		private async Task<string> InstallAppAsync()
		{
			if (string.IsNullOrEmpty(param.ApkFilePath))
			{
				return FormatAppInstallOutput(null, "Error: no package specified");
			}
			IPortableRepositoryHandler repository = null;
			correlationId = Guid.NewGuid().ToString();
			bool installedSuccessfully = false;
			EtwLogger.Instance.StartingApkInstall(correlationId, param.ApkFilePath);
			try
			{
				repository = factory.CreateRepository();
				await repository.InitializeAsync(new ApkDetails(Path.GetFileName(param.ApkFilePath)));
				EtwLogger.Instance.StartingApkSync(correlationId, param.ApkFilePath);
				PullAPkResult result = await ObtainApkAsync(param.ApkFilePath, repository.RetrievePackageFilePath());
				switch (result)
				{
					case PullAPkResult.Error_FileNotFound:
						EtwLogger.Instance.ApkSyncFailure(correlationId, string.Format(CultureInfo.InvariantCulture, "{0}, {1}", new object[2] { result, "Failure [INSTALL_FAILED_INVALID_URI]" }));
						return FormatAppInstallOutput(param.ApkFilePath, "Failure [INSTALL_FAILED_INVALID_URI]");
					default:
						EtwLogger.Instance.ApkSyncFailure(correlationId, string.Format(CultureInfo.InvariantCulture, "{0}, {1}", new object[2] { result, "Failure [INTERNAL_AGENT_ERROR]" }));
						return FormatAppInstallOutput(param.ApkFilePath, "Failure [INTERNAL_AGENT_ERROR]");
					case PullAPkResult.Success:
						{
							EtwLogger.Instance.ApkSyncSuccess(correlationId);
							ISystemInformation systemInfo = factory.CreateSystemInformation();
							if (systemInfo.Architecture == SystemArchitecture.Other)
							{
								return FormatAppInstallOutput(param.ApkFilePath, "Failure [INTERNAL_AGENT_ERROR]");
							}
							AppxPackageConfiguration packageConfig = CreateAppxConfiguration(systemInfo.Architecture);
							try
							{
								EtwLogger.Instance.ApkConverting(correlationId);
								await ConvertApk(packageConfig, repository);
								EtwLogger.Instance.ApkConverted(correlationId);
							}
							catch (ApkDecoderManifestException ex)
							{
								EtwLogger.Instance.ApkConversionFailure(correlationId, BuildAdbFailMessageFromException(ex, "Failure [MANIFEST_DECODER_ERROR]"));
								return FormatAppInstallOutput(param.ApkFilePath, "Failure [MANIFEST_DECODER_ERROR]");
							}
							catch (ApkDecoderResourcesException ex2)
							{
								EtwLogger.Instance.ApkConversionFailure(correlationId, BuildAdbFailMessageFromException(ex2, "Failure [RESOURCES_DECODER_ERROR]"));
								return FormatAppInstallOutput(param.ApkFilePath, "Failure [RESOURCES_DECODER_ERROR]");
							}
							catch (ApkDecoderConfigException ex3)
							{
								EtwLogger.Instance.ApkConversionFailure(correlationId, BuildAdbFailMessageFromException(ex3, "Failure [CONFIGURE_DECODER_ERROR]"));
								return FormatAppInstallOutput(param.ApkFilePath, "Failure [CONFIGURE_DECODER_ERROR]");
							}
							catch (DecoderManifestNoApplicationException ex4)
							{
								EtwLogger.Instance.ApkConversionFailure(correlationId, BuildAdbFailMessageFromException(ex4, "Failure [MANIFEST_DECODER_ERROR_APPLICATION_ELEMENT_NOT_FOUND]"));
								return FormatAppInstallOutput(param.ApkFilePath, "Failure [MANIFEST_DECODER_ERROR_APPLICATION_ELEMENT_NOT_FOUND]");
							}
							catch (DecoderManifestNoVersionCodeException ex5)
							{
								EtwLogger.Instance.ApkConversionFailure(correlationId, BuildAdbFailMessageFromException(ex5, "Failure [MANIFEST_DECODER_ERROR_VERSION_NOT_FOUND]"));
								return FormatAppInstallOutput(param.ApkFilePath, "Failure [MANIFEST_DECODER_ERROR_VERSION_NOT_FOUND]");
							}
							catch (ConverterException ex6)
							{
								EtwLogger.Instance.ApkConversionFailure(correlationId, BuildAdbFailMessageFromException(ex6, "Failure [CONVERTER_ERROR]"));
								return FormatAppInstallOutput(param.ApkFilePath, "Failure [CONVERTER_ERROR]");
							}
							bool reinstall = false;
							if (param.Options != null)
							{
								foreach (string option in param.Options)
								{
									if (string.Compare(option, "-r", StringComparison.Ordinal) == 0)
									{
										reinstall = true;
										break;
									}
								}
							}
							if (reinstall)
							{
								AndroidPackageUninstallService androidPackageUninstallService = new AndroidPackageUninstallService(factory);
								AndroidPackageUninstallResult result2 = androidPackageUninstallService.UninstallAndroidPackageAsync(apkModel.ManifestInfo.PackageNameResource.Content).Result;
								if (result2 != AndroidPackageUninstallResult.NotFound && result2 != AndroidPackageUninstallResult.Success)
								{
									EtwLogger.Instance.AppxUninstallFailure(correlationId, AdbMessageStrings.FromAndroidUninstallResult(result2));
									return AdbMessageStrings.FromAndroidUninstallResult(result2);
								}
							}
							EtwLogger.Instance.StartAppxInstall(correlationId);
							AppxDeployService deployer = new AppxDeployService(factory);
							PackageDeploymentResult installResult = deployer.DeployAppxProjectAsync(repository).Result;
							string formatAppInstallOutput = FormatAppInstallOutput(message: AdbMessageStrings.FromPackageManagerInstallResult(installResult), apkFilePath: param.ApkFilePath);
							if (installResult.Error == null)
							{
								EtwLogger.Instance.AppxInstalled(correlationId);
								installedSuccessfully = true;
								return formatAppInstallOutput;
							}
							EtwLogger.Instance.AppxInstallFailure(correlationId, formatAppInstallOutput);
							return formatAppInstallOutput;
						}
				}
			}
			catch (Exception ex7)
			{
				string text = BuildAdbFailMessageFromException(ex7, "Failure [INTERNAL_AGENT_ERROR]");
				EtwLogger.Instance.LogError(text);
				return text;
			}
			finally
			{
				if (repository != null && !installedSuccessfully)
				{
					try
					{
						repository.Clean();
					}
					catch (Exception exp)
					{
						LoggerCore.Log(exp);
					}
				}
			}
		}

		private async Task<PackageInformation> ConvertApk(AppxPackageConfiguration packageConfig, IPortableRepositoryHandler handler)
		{
			MobilePortableApkDecoder decoder = new MobilePortableApkDecoder(handler, correlationId);
			await decoder.DecodeAsync();
			PackageInformation package = new PackageInformation
			{
				PackageIdentityName = AppxUtilities.BuildAppxPackageIdentity(decoder.ObjModel),
				PackagePublisher = "CN=Microsoft",
				PackagePublisherDisplayName = AppxUtilities.BuildAppxPackagePublisherDisplayName()
			};
			apkModel = decoder.ObjModel;
			List<AppxPackageConfiguration> packagesConfigCollection = new List<AppxPackageConfiguration> { packageConfig };
			PackageObjectDefaults packageObjectDefaults = new PackageObjectDefaults
			{
				ApplicationNameResource = new ManifestStringResource("Test Application"),
				ApplicationIconFilePath = PortableUtilsServiceLocator.FileUtils.PathCombine(handler.RetrievePackageExtractionPath(), "msftwbfa-apktestapp.png"),
				ApplicationIconResource = new ManifestStringResource("@res:19901024"),
				ApplicationIconResourceId = 428871716u
			};
			await ExtractTestApplicationIcon(packageObjectDefaults);
			PortableApkToAppxConverter converter = new PortableApkToAppxConverter(decoder.ObjModel, handler, packagesConfigCollection, package, packageObjectDefaults);
			converter.GenerateOneAppxDirectory(packageConfig);
			return package;
		}

		private async Task<PullAPkResult> ObtainApkAsync(string apkFilePathOnAndroid, string localFilePath)
		{
			IAdbChannel fileSyncChannel = await base.Configuration.RemoteChannelManager.OpenChannelAsync("sync:\0");
			if (fileSyncChannel == null)
			{
				return PullAPkResult.Errors_Others;
			}
			try
			{
				AdbFileSyncStatPacketFromClient stat = new AdbFileSyncStatPacketFromClient(apkFilePathOnAndroid);
				if (!(await stat.WriteAsync(fileSyncChannel.StreamWriter)))
				{
					return PullAPkResult.Errors_Others;
				}
				AdbFileSyncPacket packet = await AdbFileSyncPacket.ReadAsync(fileSyncChannel.StreamReader, AdbFileSyncPacket.Direction.FromServer);
				if (packet == null)
				{
					return PullAPkResult.Errors_Others;
				}
				if (!(packet is AdbFileSyncStatPacketFromServer statResponse))
				{
					return PullAPkResult.Errors_Others;
				}
				if (statResponse.Mode == 0)
				{
					return PullAPkResult.Error_FileNotFound;
				}
				bool obtainedFile = false;
				PullAPkResult pullResult = PullAPkResult.Errors_Others;
				if (factory.AgentConfiguration.EnableInterception && TryCopyFromSnifferCache(statResponse, apkFilePathOnAndroid, localFilePath))
				{
					obtainedFile = true;
					pullResult = PullAPkResult.Success;
				}
				if (!obtainedFile)
				{
					pullResult = await SyncFromAndroid(fileSyncChannel, apkFilePathOnAndroid, localFilePath);
				}
				AdbFileSyncQuitPacket quit = new AdbFileSyncQuitPacket();
				await quit.WriteAsync(fileSyncChannel.StreamWriter);
				return pullResult;
			}
			finally
			{
				base.Configuration.RemoteChannelManager.CloseChannel(fileSyncChannel);
			}
		}

		private async Task<PullAPkResult> SyncFromAndroid(IAdbChannel fileSyncChannel, string apkFilePathOnAndroid, string localFilePath)
		{
			AdbFileSyncReceivePacket recv = new AdbFileSyncReceivePacket(apkFilePathOnAndroid);
			if (!(await recv.WriteAsync(fileSyncChannel.StreamWriter)))
			{
				return PullAPkResult.Errors_Others;
			}
			using (Stream fileStream = fileUtils.OpenOrCreateFileStream(localFilePath))
			{
				while (true)
				{
					AdbFileSyncPacket p = await AdbFileSyncPacket.ReadAsync(fileSyncChannel.StreamReader, AdbFileSyncPacket.Direction.FromServer);
					if (p != null)
					{
						if (p is AdbFileSyncDataPacket dataPacket)
						{
							await fileStream.WriteAsync(dataPacket.Data, 0, dataPacket.Data.Length);
							continue;
						}
						if (p is AdbFileSyncFailPacket)
						{
							return PullAPkResult.Errors_Others;
						}
						AdbFileSyncDonePacket donePacket = p as AdbFileSyncDonePacket;
						if (donePacket != null)
						{
							break;
						}
						continue;
					}
					return PullAPkResult.Errors_Others;
				}
			}
			return PullAPkResult.Success;
		}

		private bool TryCopyFromSnifferCache(AdbFileSyncStatPacketFromServer statResponse, string apkFilePathOnAndroid, string localFilePath)
		{
			string localDataSniffedDirectory = factory.AgentConfiguration.LocalDataSniffedDirectory;
			string linuxDirectoryName = IOUtils.GetLinuxDirectoryName(apkFilePathOnAndroid);
			LoggerCore.Log("Local Intercept Path: {0}.", localDataSniffedDirectory);
			LoggerCore.Log("Remote Linux Directory Path: {0}.", linuxDirectoryName);
			if (string.Compare(linuxDirectoryName, factory.AgentConfiguration.RemoteDataSnifferDirectory, StringComparison.Ordinal) != 0)
			{
				LoggerCore.Log("Path for pm install {0} is not on the intercept white list. Falling back to SYNC.", linuxDirectoryName);
				return false;
			}
			string folder = Path.Combine(new string[2]
			{
			localDataSniffedDirectory,
			Path.GetFileName(apkFilePathOnAndroid)
			});
			PathSanitizer pathSanitizer = new PathSanitizer(folder);
			if (!pathSanitizer.IsWithinFolder(localDataSniffedDirectory))
			{
				LoggerCore.Log("The filePath {0} does not exist within the local cache directory.", pathSanitizer.Path);
				return false;
			}
			try
			{
				if (PortableUtilsServiceLocator.FileUtils.FileExists(pathSanitizer.Path))
				{
					long fileSize = PortableUtilsServiceLocator.FileUtils.GetFileSize(pathSanitizer.Path);
					if (fileSize == statResponse.Size)
					{
						PortableUtilsServiceLocator.FileUtils.CopyFile(pathSanitizer.Path, localFilePath, overwrite: true);
						LoggerCore.Log("Successfully copy intercepted file {0} to {1}.", pathSanitizer.Path, localFilePath);
						return true;
					}
					LoggerCore.Log("Mismatch between file in Android Environment and copy in cache directory.");
				}
			}
			catch (Exception exp)
			{
				if (!ExceptionUtils.IsIOException(exp))
				{
					throw;
				}
				LoggerCore.Log(exp);
			}
			return false;
		}

		[SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "Microsoft.Arcadia.Debugging.AdbAgent.Portable.ApkInstallJob.FormatAppInstallOutput(System.String,System.String)", Justification = "Reviewed.")]
		private string BuildAdbFailMessageFromException(Exception ex, string reasonForFailure)
		{
			return FormatAppInstallOutput(message: (!NativeMethods.IsDiskspaceFullException(ex)) ? reasonForFailure : "Failure [OUT_OF_DISK_SPACE]", apkFilePath: param.ApkFilePath);
		}

		private async Task ExtractTestApplicationIcon(PackageObjectDefaults defaults)
		{
			Assembly currentAssembly = typeof(ApkInstallJob).GetTypeInfo().Assembly;
			using (Stream resourceStream = currentAssembly.GetManifestResourceStream("Microsoft.Arcadia.Debugging.AdbAgent.Portable.apktestapp.png"))
			{
				if (resourceStream == null)
				{
					throw new InvalidOperationException("Could not find test application icon in resources.");
				}
				using (Stream fileStream = PortableUtilsServiceLocator.FileUtils.OpenOrCreateFileStream(defaults.ApplicationIconFilePath))
				{
					await resourceStream.CopyToAsync(fileStream);
				}
			}
		}
	}
}