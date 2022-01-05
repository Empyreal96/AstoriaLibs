using System.Collections.Generic;

namespace Microsoft.Arcadia.Marketplace.PackageObjectModel.Apk
{
	public interface IDevReportManifestObjectModel
	{
		int MinSdkVersion { get; }

		int TargetSdkVersion { get; }

		int? MaxSdkVersion { get; }

		string PackageName { get; }

		string ActualPackageName { get; }

		string VersionCodeValue { get; }

		IDevReportManifestApplication ManifestApplication { get; }

		IReadOnlyCollection<string> AllPermissions { get; }

		IReadOnlyCollection<string> AllUsesFeatures { get; }

		IReadOnlyCollection<IDevReportActivity> AllActivities { get; }

		IReadOnlyCollection<IDevReportActivityAlias> AllActivityAliases { get; }

		IReadOnlyCollection<IDevReportReceiver> AllReceivers { get; }

		IReadOnlyCollection<IDevReportManifestService> AllServices { get; }

		IReadOnlyCollection<IDevReportActivity> ContradictingActivities { get; }
	}
}