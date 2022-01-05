namespace Microsoft.Arcadia.Marketplace.PackageObjectModel.Apk
{
	public interface IDevReportActivity : IDevReportIntentReceiver, IDevReportMetadataContainer
	{
		string NameString { get; }

		bool HasMainActivity { get; }

		bool IsLauncherCategory { get; }

		bool IsHomeCategory { get; }

		string LaunchModeValue { get; }
	}
}