using System;
using System.Collections.Generic;
using Microsoft.Arcadia.Marketplace.PackageObjectModel.Apk;

namespace Microsoft.Arcadia.Marketplace.PackageObjectModel.Portable.Apk
{
	public static class ManifestObjectModelExtensions
	{
		public static void InjectTestIconResource(this ApkObjectModel model, uint iconResourceId, string iconFileName)
		{
			if (model == null)
			{
				throw new ArgumentNullException("model");
			}
			if (string.IsNullOrWhiteSpace(iconFileName))
			{
				throw new ArgumentException("iconFileName cannot be empty.", "iconFileName");
			}
			ApkResourceConfig apkResourceConfig = new ApkResourceConfig();
			apkResourceConfig.Locale = null;
			apkResourceConfig.Unsupported = false;
			apkResourceConfig.TypeSpecEntry = 255u;
			ApkResourceConfig config = apkResourceConfig;
			ApkResourceValue item = new ApkResourceValue(ApkResourceType.Drawable, config, iconFileName);
			List<ApkResourceValue> list = new List<ApkResourceValue>();
			list.Add(item);
			List<ApkResourceValue> values = list;
			ApkResource value = new ApkResource(values, ApkResourceType.Drawable);
			model.Resources.Add(new KeyValuePair<uint, ApkResource>(iconResourceId, value));
		}
	}
}