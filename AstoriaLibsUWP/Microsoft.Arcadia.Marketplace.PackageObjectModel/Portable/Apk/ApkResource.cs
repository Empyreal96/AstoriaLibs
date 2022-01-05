using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Microsoft.Arcadia.Marketplace.PackageObjectModel.Portable.Apk
{
	public sealed class ApkResource
	{
		public const string ResourcesFileName = "Resources.arsc";

		private List<ApkResourceValue> values;

		public IReadOnlyList<ApkResourceValue> Values => values.ToList();

		public ApkResourceType ResourceType { get; private set; }

		public ApkResource(IEnumerable<ApkResourceValue> values, ApkResourceType resourceType)
		{
			if (values == null)
			{
				throw new ArgumentNullException("values");
			}
			this.values = new List<ApkResourceValue>();
			foreach (ApkResourceValue value in values)
			{
				if (value.ResourceType != resourceType)
				{
					throw new PackageObjectModelException(string.Format(CultureInfo.InvariantCulture, "Resource Type should be {0}, but found as {1}", new object[2] { resourceType, value.ResourceType }));
				}
				this.values.Add(value);
			}
			ResourceType = resourceType;
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder(string.Format(CultureInfo.InvariantCulture, "ApkResource - ApkResourceValue Count: {0}", new object[1] { Values.Count }));
			stringBuilder.AppendFormat(CultureInfo.InvariantCulture, "\nApkResourceValue Items:\n");
			foreach (ApkResourceValue value in Values)
			{
				stringBuilder.AppendFormat(CultureInfo.InvariantCulture, "{0}\n", new object[1] { value });
			}
			return stringBuilder.ToString();
		}

		public void AddResource(ApkResourceValue oneResourceValue)
		{
			if (oneResourceValue == null)
			{
				throw new ArgumentNullException("oneResourceValue");
			}
			values.Add(oneResourceValue);
		}

		public void RemoveResource(ApkResourceValue oneResourceValue)
		{
			if (oneResourceValue == null)
			{
				throw new ArgumentNullException("oneResourceValue");
			}
			if (values.Contains(oneResourceValue))
			{
				values.Remove(oneResourceValue);
			}
		}
	}
}