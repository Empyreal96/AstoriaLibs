using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.Arcadia.Marketplace.PackageObjectModel.Apk
{
	public interface IDevReportServicesModel
	{
		[SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "Purposefully encapsulated to allow ease of review by external stake holders.")]
		IReadOnlyDictionary<ServiceName, ICollection<CalledTrackedServiceMethodDetails>> ServicesDetails { get; }
	}
}