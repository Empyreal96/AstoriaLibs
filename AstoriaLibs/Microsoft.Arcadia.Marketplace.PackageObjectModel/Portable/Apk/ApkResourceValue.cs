using System.Collections.Generic;
using System.Globalization;

namespace Microsoft.Arcadia.Marketplace.PackageObjectModel.Portable.Apk
{
	public sealed class ApkResourceValue
	{
		public ApkResourceConfig Config { get; private set; }

		public string Value { get; private set; }

		public ApkResourceType ResourceType { get; private set; }

		public ApkResourceValue(ApkResourceType type, ApkResourceConfig config, string value)
		{
			ResourceType = type;
			Config = config;
			Value = value;
		}

		public void ResolveResourceValue(IDictionary<uint, ApkResource> resources)
		{
			if (resources == null)
			{
				return;
			}
			int num = 1;
			while (num <= 3)
			{
				ManifestStringResource manifestStringResource = new ManifestStringResource(Value);
				if (!manifestStringResource.IsResource || !resources.ContainsKey(manifestStringResource.ResourceId) || resources[manifestStringResource.ResourceId].Values.Count <= 0)
				{
					break;
				}
				if (!string.IsNullOrEmpty(Config.Locale))
				{
					bool flag = false;
					foreach (ApkResourceValue value in resources[manifestStringResource.ResourceId].Values)
					{
						if (Config.Locale.Equals(value.Config.Locale))
						{
							Value = value.Value;
							num++;
							flag = true;
							break;
						}
					}
					if (flag)
					{
						continue;
					}
				}
				Value = resources[manifestStringResource.ResourceId].Values[0].Value;
				num++;
			}
		}

		public override string ToString()
		{
			return string.Format(CultureInfo.InvariantCulture, "ApkResourceValue - Config: {0}, Value: {1}, ApkResourceType: {2}", new object[3] { Config, Value, ResourceType });
		}
	}
}