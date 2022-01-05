using System.Collections.Generic;

namespace Microsoft.Arcadia.Marketplace.PackageObjectModel.Apk
{
	public interface IDevReportMiddlewareModel
	{
		IReadOnlyCollection<Middleware> ReferencedMiddlewareList { get; }

		IReadOnlyCollection<string> TrackedMiddlewareNamespaceList { get; }
	}
}