using System.Collections.Generic;

namespace Microsoft.Arcadia.Marketplace.PackageObjectModel.Apk
{
	public interface IDevReportIntentFilter
	{
		IReadOnlyList<string> Actions { get; }

		IReadOnlyList<IDevReportIntentFilterData> FilterData { get; }
	}
}