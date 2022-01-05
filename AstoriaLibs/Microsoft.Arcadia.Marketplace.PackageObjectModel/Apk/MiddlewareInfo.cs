using System.Globalization;

namespace Microsoft.Arcadia.Marketplace.PackageObjectModel.Apk
{
	public class MiddlewareInfo
	{
		public string Name { get; set; }

		public string Namespace { get; set; }

		public string Category { get; set; }

		public string Website { get; set; }

		public string Description { get; set; }

		public string License { get; set; }

		public MiddlewareInfo()
		{
		}

		public MiddlewareInfo(string name, string namespaceValue, string category, string website, string description, string license)
		{
			Name = name;
			Namespace = namespaceValue;
			Category = category;
			Website = website;
			Description = description;
			License = license;
		}

		public override string ToString()
		{
			return string.Format(CultureInfo.InvariantCulture, "{0},{1},{2},{3},{4},{5}", Name, Namespace, Category, Website, Description, License);
		}
	}
}