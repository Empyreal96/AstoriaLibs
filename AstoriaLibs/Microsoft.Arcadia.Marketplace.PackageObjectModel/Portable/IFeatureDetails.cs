using System;

namespace Microsoft.Arcadia.Marketplace.PackageObjectModel.Portable
{
	public interface IFeatureDetails
	{
		IReportBuilder ReportBuilder { get; }

		Enum Message { get; }

		IFeatureLog CreateFeatureLog();

		AggregateFeature CreateAggregateFeature();
	}
}