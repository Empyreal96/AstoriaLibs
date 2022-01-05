using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Arcadia.Debugging.AdbAgent.Portable;
using Microsoft.Arcadia.Debugging.SharedUtils.Portable;
using Microsoft.Arcadia.Marketplace.Utils.Log;
using Windows.ApplicationModel;
using Windows.Foundation;
using Windows.Management.Deployment;

namespace Microsoft.Arcadia.Debugging.AdbAgent.Mobile
{
	public class PackageManagerMobile : IPackageManager
	{
		private const int RetryAttempts = 4;

		private const int InstallRetryAttemptSleep = 1500;

		private const uint ErrorInstallFailed = 2147958009u;

		private readonly string currentUserSid = GetCurrentUserSidAsString();

		private PackageManager pacman = new PackageManager();

		public IList<AppxPackage> FindPackages()
		{
			IList<AppxPackage> list = new List<AppxPackage>();
			IEnumerable<Package> enumerable = pacman.FindPackagesForUser(currentUserSid);
			foreach (Package item in enumerable)
			{
				list.Add(new AppxPackage(item.Id.Name, item.Id.FullName, item.Id.Publisher, ReadPackageLocation(item)));
			}
			return list;
		}

		public async Task<PackageDeploymentResult> InstallAppFromFolderLayoutAsync(Uri manifestUri, IEnumerable<Uri> dependencyPackageUris)
		{
			if (manifestUri == null)
			{
				throw new ArgumentNullException("manifestUri");
			}
			int retryAttempt = 0;
			PackageDeploymentResult lastPackageDeploymentResult = null;
			for (; retryAttempt <= 4; retryAttempt++)
			{
				if (retryAttempt > 0)
				{
					LoggerCore.Log("Sleeping {0} millisecond(s)...", 1500);
					await Task.Delay(1500);
					LoggerCore.Log("Woke up!");
					EtwLogger.Instance.AppxInstallReattempt();
				}
				IAsyncOperationWithProgress<DeploymentResult, DeploymentProgress> deploymentOperation = pacman.RegisterPackageAsync(manifestUri, (IEnumerable<Uri>)null, (DeploymentOptions)3);
				ManualResetEvent deploymentCompletedEvent = new ManualResetEvent(initialState: false);
				try
				{
					deploymentOperation.Completed = ((AsyncOperationWithProgressCompletedHandler<DeploymentResult, DeploymentProgress>)(object)Delegate.Combine((Delegate)(object)deploymentOperation.Completed, (Delegate)(object)(AsyncOperationWithProgressCompletedHandler<DeploymentResult, DeploymentProgress>)delegate
					{
						deploymentCompletedEvent.Set();
					}));
					deploymentCompletedEvent.WaitOne();
				}
				finally
				{
					if (deploymentCompletedEvent != null)
					{
						((IDisposable)deploymentCompletedEvent).Dispose();
					}
				}
				lastPackageDeploymentResult = new PackageDeploymentResult(extendedError: deploymentOperation.GetResults().ExtendedErrorCode, error: ((IAsyncInfo)deploymentOperation).ErrorCode);
				if (lastPackageDeploymentResult.Error == null || lastPackageDeploymentResult.Error.HResult != -2147009287)
				{
					break;
				}
				LoggerCore.Log(LoggerCore.LogLevels.Info, "Pacman returned ERROR_INSTALL_FAILED");
			}
			if (retryAttempt >= 5)
			{
				LoggerCore.Log(LoggerCore.LogLevels.Info, "Installation retry attempts exhausted.");
			}
			return lastPackageDeploymentResult;
		}

		public async Task<AppUninstallResult> UninstallAppsAsync(AppxPackage package)
		{
			if (package == null)
			{
				throw new ArgumentNullException("package");
			}
			return await UninstallAppsAsync(package.PackageName, package.PackagePublisher);
		}

		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "By Design.")]
		private static string ReadPackageLocation(Package package)
		{
			string result = null;
			try
			{
				result = package.InstalledLocation.Path;
				return result;
			}
			catch (Exception exp)
			{
				LoggerCore.Log(exp);
				return result;
			}
		}

		private static string GetCurrentUserSidAsString()
		{
			bool flag = false;
			IntPtr intPtr = IntPtr.Zero;
			IntPtr TokenHandle = IntPtr.Zero;
			IntPtr intPtr2 = IntPtr.Zero;
			try
			{
				intPtr = NativeMethods.GetCurrentProcess();
				if (!NativeMethods.OpenProcessToken(intPtr, 8, out TokenHandle))
				{
					LoggerCore.Log(LoggerCore.LogLevels.Error, "OpenProcessToken Failed");
					Marshal.ThrowExceptionForHR(Marshal.GetHRForLastWin32Error());
				}
				int reqLength = -1;
				if (!NativeMethods.GetTokenInformation(TokenHandle, NativeMethods.TOKEN_INFORMATION_CLASS.TokenUser, IntPtr.Zero, 0, out reqLength))
				{
					if (reqLength > 0)
					{
						LoggerCore.Log("GetTokenInformation first call got the token length: {0}.", reqLength);
					}
					else
					{
						LoggerCore.Log(LoggerCore.LogLevels.Error, "GetTokenInformation first call failed, error code = {0}.", Marshal.GetLastWin32Error());
						Marshal.ThrowExceptionForHR(Marshal.GetHRForLastWin32Error());
					}
				}
				intPtr2 = Marshal.AllocHGlobal(reqLength);
				if (!NativeMethods.GetTokenInformation(TokenHandle, NativeMethods.TOKEN_INFORMATION_CLASS.TokenUser, intPtr2, reqLength, out reqLength))
				{
					LoggerCore.Log(LoggerCore.LogLevels.Error, "GetTokenInformation second call Failed");
					Marshal.ThrowExceptionForHR(Marshal.GetHRForLastWin32Error());
				}
				if (!NativeMethods.ConvertSidToStringSid(Marshal.PtrToStructure<NativeMethods.TOKEN_USER>(intPtr2).User.Sid, out var pStringSid))
				{
					LoggerCore.Log(LoggerCore.LogLevels.Error, "ConvertSidToStringSid Failed");
					Marshal.ThrowExceptionForHR(Marshal.GetHRForLastWin32Error());
				}
				return pStringSid;
			}
			finally
			{
				if (intPtr2 != IntPtr.Zero)
				{
					Marshal.FreeHGlobal(intPtr2);
				}
				if (TokenHandle != IntPtr.Zero)
				{
					NativeMethods.CloseHandle(TokenHandle);
				}
				if (intPtr != IntPtr.Zero)
				{
					NativeMethods.CloseHandle(intPtr);
				}
			}
		}

		private async Task<AppUninstallResult> UninstallAppsAsync(string packageName, string packagePublisher)
		{
			if (string.IsNullOrWhiteSpace(packageName))
			{
				throw new ArgumentException("Package name cannot be null or whitespace.", "packageName");
			}
			if (string.IsNullOrWhiteSpace(packagePublisher))
			{
				throw new ArgumentException("Package publisher cannot be null or whitespace.", "packagePublisher");
			}
			IEnumerable<Package> packages = pacman.FindPackagesForUser(currentUserSid);
			int foundCount = 0;
			foreach (Package package in packages)
			{
				if (string.Compare(package.Id.Name, packageName, StringComparison.Ordinal) == 0 && string.Compare(package.Id.Publisher, packagePublisher, StringComparison.Ordinal) == 0)
				{
					foundCount++;
					PackageDeploymentResult result = await UninstallAppsAsync(package.Id.FullName);
					if (result.Error != null)
					{
						return AppUninstallResult.Error;
					}
				}
			}
			return (foundCount <= 0) ? AppUninstallResult.NotFound : AppUninstallResult.Success;
		}

		private async Task<PackageDeploymentResult> UninstallAppsAsync(string fullName)
		{
			LoggerCore.Log("Uninstalling " + fullName);
			IAsyncOperationWithProgress<DeploymentResult, DeploymentProgress> val = pacman.RemovePackageAsync(fullName);
			ManualResetEvent deploymentCompletedEvent = new ManualResetEvent(initialState: false);
			try
			{
				val.Completed = ((AsyncOperationWithProgressCompletedHandler<DeploymentResult, DeploymentProgress>)(object)Delegate.Combine((Delegate)(object)val.Completed, (Delegate)(object)(AsyncOperationWithProgressCompletedHandler<DeploymentResult, DeploymentProgress>)delegate
				{
					deploymentCompletedEvent.Set();
				}));
				deploymentCompletedEvent.WaitOne();
			}
			finally
			{
				if (deploymentCompletedEvent != null)
				{
					((IDisposable)deploymentCompletedEvent).Dispose();
				}
			}
			DeploymentResult results = val.GetResults();
			if (((IAsyncInfo)val).ErrorCode == null)
			{
				LoggerCore.Log("Uninstall succeeded");
			}
			else
			{
				LoggerCore.Log("Uninstall failed");
				LoggerCore.Log(LoggerCore.LogLevels.Error, ((IAsyncInfo)val).ErrorCode);
				if (!string.IsNullOrWhiteSpace(results.ErrorText))
				{
					LoggerCore.Log(LoggerCore.LogLevels.Error, results.ErrorText);
				}
				if (results.ExtendedErrorCode != null)
				{
					LoggerCore.Log(LoggerCore.LogLevels.Error, results.ExtendedErrorCode);
				}
			}
			return new PackageDeploymentResult(((IAsyncInfo)val).ErrorCode, results.ExtendedErrorCode);
		}
	}
}
