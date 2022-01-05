using System.Globalization;

namespace Microsoft.Arcadia.Marketplace.PackageObjectModel.Portable.Apk
{
	public sealed class ApkResourceConfig
	{
		public uint TypeSpecEntry { get; set; }

		public string Locale { get; set; }

		public bool Unsupported { get; set; }

		public ApkResourceConfig()
		{
			Locale = null;
			Unsupported = false;
			TypeSpecEntry = 0u;
		}

		public override string ToString()
		{
			return string.Format(CultureInfo.InvariantCulture, "ApkResourceConfig - Locale: {0}", new object[1] { Locale });
		}
	}
}