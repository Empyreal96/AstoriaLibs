using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.Arcadia.Marketplace.PackageObjectModel.Portable;
using Microsoft.Arcadia.Marketplace.PackageObjectModel.Portable.Apk;
using Microsoft.Arcadia.Marketplace.Utils.Log;

namespace Microsoft.Arcadia.Marketplace.PackageObjectModel.Apk
{
	public static class ApkResourceHelper
	{
		private static IReadOnlyList<string> acceptableDrawableFormat = new List<string> { ".PNG", ".JPG", ".GIF" };

		public static string GetRelativeDrawableFilePath(ApkResourceValue resourceVal, IDictionary<uint, ApkResource> apkResources)
		{
			if (resourceVal == null)
			{
				throw new ArgumentNullException("resourceVal");
			}
			resourceVal.ResolveResourceValue(apkResources);
			string value = resourceVal.Value;
			LoggerCore.Log("Relative Drawable File Path: {0}", value);
			string extension = Path.GetExtension(value.ToUpperInvariant());
			if (!acceptableDrawableFormat.Contains(extension))
			{
				return string.Empty;
			}
			return value;
		}

		public static IEnumerable<string> FindResourcesMatchingPattern(ApkResourceType resourceType, Regex searchPattern, IDictionary<uint, ApkResource> resources)
		{
			foreach (ApkResource resource2 in resources.Values.Where((ApkResource resource) => resource.ResourceType == resourceType))
			{
				foreach (ApkResourceValue currentValue in resource2.Values)
				{
					Match resultMatch = searchPattern.Match(currentValue.Value);
					if (resultMatch != Match.Empty)
					{
						yield return resultMatch.Value;
					}
				}
			}
		}

		public static ApkResource GetResource(ManifestStringResource manifestValue, IDictionary<uint, ApkResource> resources)
		{
			if (manifestValue == null)
			{
				throw new ArgumentNullException("manifestValue");
			}
			if (resources == null)
			{
				throw new ArgumentNullException("resources");
			}
			ApkResource value = null;
			if (manifestValue.IsResource && !resources.TryGetValue(manifestValue.ResourceId, out value))
			{
				value = null;
			}
			if (value == null)
			{
				throw new PackageObjectModelException("The field is expected to be a reference to a resource");
			}
			foreach (ApkResourceValue value2 in value.Values)
			{
				value2.ResolveResourceValue(resources);
			}
			return value;
		}

		public static void ResolveAllStringResourcesAsDrawable(ApkObjectModel apkObjectModel, ApkResource apkResource)
		{
			if (apkObjectModel == null)
			{
				throw new ArgumentNullException("apkObjectModel");
			}
			if (apkResource == null)
			{
				throw new ArgumentNullException("apkResource");
			}
			List<ApkResourceValue> list = new List<ApkResourceValue>();
			List<ApkResourceValue> list2 = new List<ApkResourceValue>();
			foreach (ApkResourceValue value in apkResource.Values)
			{
				if (value.ResourceType != ApkResourceType.String)
				{
					continue;
				}
				ManifestStringResource manifestStringResource = new ManifestStringResource(value.Value);
				if (manifestStringResource.IsResource)
				{
					ApkResource resource = GetResource(manifestStringResource, apkObjectModel.Resources);
					foreach (ApkResourceValue value2 in resource.Values)
					{
						if (value2.ResourceType == ApkResourceType.Drawable)
						{
							if (!list.Contains(value2))
							{
								list.Add(value2);
							}
							continue;
						}
						throw new PackageObjectModelException("Non-drawable resource found while resolving an indirect drawable resource. Currently multiple levels of redirection is not supported.");
					}
				}
				list2.Add(value);
			}
			foreach (ApkResourceValue item in list)
			{
				apkResource.AddResource(item);
			}
			foreach (ApkResourceValue item2 in list2)
			{
				apkResource.RemoveResource(item2);
			}
		}
	}
}