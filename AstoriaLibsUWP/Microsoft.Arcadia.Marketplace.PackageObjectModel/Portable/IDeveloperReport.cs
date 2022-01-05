namespace Microsoft.Arcadia.Marketplace.PackageObjectModel.Portable
{
	public interface IDeveloperReport
	{
		void AddReportMessage(IFeatureDetails featureDetails);

		void AddNewReportMessage(IFeatureDetails featureDetails);
	}
}