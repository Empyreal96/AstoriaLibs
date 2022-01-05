namespace Microsoft.Arcadia.Marketplace.PackageObjectModel.Apk
{
	public interface IDevReportActivityAlias : IDevReportIntentReceiver, IDevReportMetadataContainer
	{
		bool HasMainActivity { get; }

		bool IsHomeCategory { get; }

		bool IsLauncherCategory { get; }

		string TargetActivityString { get; }
	}
}