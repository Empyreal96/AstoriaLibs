using System;
using Microsoft.Arcadia.Marketplace.PackageObjectModel.Apk;
using Microsoft.Arcadia.Marketplace.PackageObjectModel.Portable.Apk;

namespace Microsoft.Arcadia.Marketplace.PackageObjectModel.Portable.Appx
{
	public static class AppxUtilities
	{
		private const int AppxPackageNameMaxLength = 35;

		public static string BuildAppxPackageIdentity(ApkObjectModel apkModel)
		{
			if (apkModel == null)
			{
				throw new ArgumentNullException("apkModel");
			}
			string apkPackageName = ExtractPackageNameString(apkModel);
			return BuildAppxPackageIdentity(apkPackageName);
		}

		public static string BuildAppxPackagePublisherDisplayName()
		{
			return "Developer";
		}

		public static string BuildAppxPackageIdentity(string apkPackageName)
		{
			if (apkPackageName == null)
			{
				throw new ArgumentNullException("apkPackageName");
			}
			string text = SanitizePackageName(apkPackageName);
			return "Aow" + text;
		}

		private static string SanitizePackageName(string packageName)
		{
			char[] array = new char[4] { ' ', '_', '-', '.' };
			if (string.IsNullOrWhiteSpace(packageName))
			{
				throw new InvalidOperationException("Package name is empty or null.");
			}
			char[] array2 = array;
			foreach (char c in array2)
			{
				packageName = packageName.Replace(c.ToString(), string.Empty);
			}
			if (packageName.Length > 35)
			{
				packageName = packageName.Substring(0, 35);
			}
			return packageName;
		}

		private static string ExtractPackageNameString(ApkObjectModel apkModel)
		{
			string result = apkModel.ManifestInfo.PackageNameResource.Content;
			if (apkModel.ManifestInfo.PackageNameResource.IsResource)
			{
				ApkResource resource = ApkResourceHelper.GetResource(apkModel.ManifestInfo.PackageNameResource, apkModel.Resources);
				if (resource.Values.Count <= 0)
				{
					throw new InvalidOperationException("No resource entry for the package name.");
				}
				result = resource.Values[0].Value;
			}
			return result;
		}
	}
}