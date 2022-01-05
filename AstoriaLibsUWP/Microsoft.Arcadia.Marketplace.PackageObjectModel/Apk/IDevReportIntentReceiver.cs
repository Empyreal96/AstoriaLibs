using System.Collections.Generic;

namespace Microsoft.Arcadia.Marketplace.PackageObjectModel.Apk
{
	public interface IDevReportIntentReceiver
	{
		IReadOnlyCollection<IDevReportIntentFilter> IntentFilters { get; }
	}
}