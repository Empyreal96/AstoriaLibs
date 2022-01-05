using System;
using System.Threading.Tasks;

namespace Microsoft.Arcadia.Debugging.AdbAgent.Portable;

internal class ApkUninstallJob : ShellChannelJob
{
	private IFactory factory;

	private ShellPmUninstallParam uninstallParameters;

	public ApkUninstallJob(IFactory factory, ShellPmUninstallParam uninstallParams)
	{
		if (factory == null)
		{
			throw new ArgumentNullException("factory");
		}
		if (uninstallParams == null)
		{
			throw new ArgumentNullException("uninstallParams");
		}
		this.factory = factory;
		uninstallParameters = uninstallParams;
		base.IsWithinInteractiveShell = uninstallParams.FromInteractiveShell;
	}

	protected override async Task<string> OnExecuteShellCommand()
	{
		if (!uninstallParameters.IsPackageNameSpecified)
		{
			return "Failure [PACKAGE_INVALID_NAME]";
		}
		AndroidPackageUninstallService uninstallService = new AndroidPackageUninstallService(factory);
		return AdbMessageStrings.FromAndroidUninstallResult(await uninstallService.UninstallAndroidPackageAsync(uninstallParameters.PackageName));
	}
}
