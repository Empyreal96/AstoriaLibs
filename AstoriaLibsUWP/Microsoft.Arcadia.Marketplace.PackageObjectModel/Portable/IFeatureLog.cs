using System.Globalization;

namespace Microsoft.Arcadia.Marketplace.PackageObjectModel.Portable
{
	public interface IFeatureLog
	{
		bool VisibleInReport { get; }

		ReportSectionH1 SectionH1 { get; }

		ReportSectionH2 SectionH2 { get; }

		WorkerLogProvider FeatureProvider { get; }

		string ProviderName { get; set; }

		string MessageCode { get; }

		bool IsSuppressed { get; set; }

		WorkerLogLevel LogLevel { get; }

		string FeatureSignature { get; }

		int NumberOfFields { get; }

		string GetFieldText(int fieldIndex, CultureInfo culture);

		object[] GetFieldArguments(int fieldIndex);

		string GetFieldTitle(int fieldIndex, CultureInfo culture);
	}
}