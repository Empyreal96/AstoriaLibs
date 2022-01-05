using System.Collections.Generic;

namespace Microsoft.Arcadia.Marketplace.PackageObjectModel.Apk
{
	public interface IDevReportMetadataContainer
	{
		IReadOnlyCollection<DevReportMetadata> Metadata { get; }
	}
}