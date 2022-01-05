using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Arcadia.Debugging.AdbAgent.Portable.Exceptions;
using Microsoft.Arcadia.Marketplace.Utils.Log;

namespace Microsoft.Arcadia.Debugging.AdbAgent.Portable
{
	internal class AndroidPackageResolverService
	{
		private IFactory factory;

		public AndroidPackageResolverService(IFactory factory)
		{
			if (factory == null)
			{
				throw new ArgumentNullException("factory");
			}
			this.factory = factory;
		}

		public AppxPackage ResolveAppxFromAndroidPackage(string androidPackageName)
		{
			if (string.IsNullOrWhiteSpace(androidPackageName))
			{
				throw new ArgumentNullException("androidPackageName");
			}
			try
			{
				string text = factory.AowInstance.AndroidPackageToWindowsPackage(androidPackageName);
				if (text == null)
				{
					LoggerCore.Log("Unable to resolve {0} to a Windows Appx Package.", androidPackageName);
					return null;
				}
				LoggerCore.Log("Android Package Name: {0}.", androidPackageName);
				LoggerCore.Log("Resolved APPX Full Package Name: {0}.", text);
				return FindPackageFromFullName(text);
			}
			catch (Exception ex)
			{
				LoggerCore.Log(ex);
				throw new AndroidPackageResolveException(ex);
			}
		}

		private AppxPackage FindPackageFromFullName(string appxFullName)
		{
			IPackageManager packageManager = factory.CreatePackageManager();
			IEnumerable<AppxPackage> source = from m in packageManager.FindPackages()
											  where string.Compare(m.PackageFullName, appxFullName, StringComparison.Ordinal) == 0
											  select m;
			if (source.Count() > 1)
			{
				throw new InvalidOperationException("Unexpected number of packages returned.");
			}
			return source.FirstOrDefault();
		}
	}
}
