using Microsoft.Arcadia.Marketplace.PackageObjectModel.Apk;

namespace Microsoft.Arcadia.Marketplace.PackageObjectModel.Portable
{
	public interface IReportBuilder
	{
		void BuildReport(IDevReportObjectModel devReportObjectModel, IDeveloperReport report);
	}
}