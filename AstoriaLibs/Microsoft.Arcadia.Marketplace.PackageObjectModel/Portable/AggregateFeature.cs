namespace Microsoft.Arcadia.Marketplace.PackageObjectModel.Portable
{
	public class AggregateFeature
	{
		public string AggregateFeatureName { get; private set; }

		public uint MessageVersion { get; private set; }

		public AggregateFeature(string aggregateFeature, uint messageVersion)
		{
			AggregateFeatureName = aggregateFeature;
			MessageVersion = messageVersion;
		}
	}
}