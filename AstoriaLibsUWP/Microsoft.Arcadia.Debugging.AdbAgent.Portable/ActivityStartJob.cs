using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Arcadia.Debugging.AdbAgent.Portable.Exceptions;
using Microsoft.Arcadia.Marketplace.Utils.Log;
using Microsoft.Arcadia.Marketplace.Utils.Portable;

namespace Microsoft.Arcadia.Debugging.AdbAgent.Portable
{
	internal class ActivityStartJob : ShellChannelJob
	{
		private IFactory factory;

		private ShellAmStartParam startParameters;

		public ActivityStartJob(IFactory factory, ShellAmStartParam startParameters)
		{
			if (factory == null)
			{
				throw new ArgumentNullException("factory");
			}
			if (startParameters == null)
			{
				throw new ArgumentNullException("startParameters");
			}
			this.factory = factory;
			this.startParameters = startParameters;
			base.IsWithinInteractiveShell = startParameters.FromInteractiveShell;
		}

		protected override async Task<string> OnExecuteShellCommand()
		{
			Intent intent = startParameters.Intent;
			if (string.Compare(intent.Action, "android.intent.action.DELETE", StringComparison.Ordinal) == 0)
			{
				return await RemovePackage();
			}
			IShellManager lockscreenManager = factory.CreateShellManager();
			if (lockscreenManager.IsScreenLocked)
			{
				return "Failure [SCREEN_LOCKED]";
			}
			if (!startParameters.IntentPresent)
			{
				return "Failure [INTENT_NOT_SPECIFIED]";
			}
			if (intent.IsUnsupportedIntent)
			{
				return "Failure [INTENT_NOT_SUPPORTED]";
			}
			LoggerCore.Log("Received request to start activity:");
			LoggerCore.Log("    IsExplicitIntent: {0}", intent.IsExplicitIntent);
			LoggerCore.Log("    Package Name: {0}", intent.PackageName);
			LoggerCore.Log("    Intent Category: {0}", intent.Category);
			LoggerCore.Log("    Intent Action: {0}", intent.Action);
			AndroidPackageResolverService resolver = new AndroidPackageResolverService(factory);
			AppxPackage resolvedPackage = resolver.ResolveAppxFromAndroidPackage(intent.PackageName);
			if (resolvedPackage != null)
			{
				return await StartActivity();
			}
			return "Failure [PACKAGE_NOT_FOUND]";
		}

		private async Task<string> RemovePackage()
		{
			Intent intent = startParameters.Intent;
			if (!intent.HasDataFlag)
			{
				return "Failure [MISSING_PACKAGE_URI]";
			}
			if (string.Compare(intent.DataUri.Scheme, "package", StringComparison.Ordinal) != 0)
			{
				return "Failure [MALFORMED_PACKAGE_URI]";
			}
			string packageName = intent.DataUri.AbsolutePath;
			Regex packageNameRegex = new Regex("^([a-z0-9\\._]+)$");
			if (!packageNameRegex.IsMatch(packageName))
			{
				return "Failure [MALFORMED_PACKAGE_URI]";
			}
			AndroidPackageUninstallService uninstallService = new AndroidPackageUninstallService(factory);
			return AdbMessageStrings.FromAndroidUninstallResult(await uninstallService.UninstallAndroidPackageAsync(packageName));
		}

		private async Task<string> StartActivity()
		{
			Uri launchUri = BuildLaunchUri();
			LoggerCore.Log("Launch URI is: {0}", launchUri.AbsoluteUri);
			try
			{
				await factory.CreateUriLauncher().LaunchUri(launchUri);
				return "Success";
			}
			catch (LauncherUriException exp)
			{
				LoggerCore.Log(exp);
				return "Failure [INTENT_START_ERROR]";
			}
		}

		[SuppressMessage("Microsoft.Globalization", "CA1308:NormalizeStringsToUppercase", Justification = "Must be lowercase to match manifest registration.")]
		private Uri BuildLaunchUri()
		{
			Intent intent = startParameters.Intent;
			byte[] bytes = Encoding.UTF8.GetBytes(intent.PackageName.ToLowerInvariant());
			string text = CryptoHelper.ComputeMD5HashAsHexadecimal(bytes).ToLowerInvariant();
			string text2 = Uri.EscapeUriString(intent.PackageName);
			string text3 = Uri.EscapeUriString(intent.ActivityName);
			string text4 = Uri.EscapeUriString(intent.Action);
			string text5 = Uri.EscapeUriString(intent.Category);
			string uriString = string.Format(CultureInfo.InvariantCulture, "a+{0}://{1}/{2}?action={3}&category={4}&debug={5}", text, text2, text3, text4, text5, startParameters.IsDebugging.ToString().ToLower());
			return new Uri(uriString);
		}
	}
}
