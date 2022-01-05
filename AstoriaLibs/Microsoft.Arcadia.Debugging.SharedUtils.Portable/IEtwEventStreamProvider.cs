namespace Microsoft.Arcadia.Debugging.SharedUtils.Portable
{
	public interface IEtwEventStreamProvider
	{
		string SessionIdentifier { get; set; }

		void Startup();

		void AdbDaemonConnected();

		void AdbDaemonConnectFailure();

		void ListeningForAdbServer();

		void AdbServerAccepted();

		void SocketDataSendReceiveError(string socketIdentifier, string reason);

		void ForcefullyClosedDaemonSocket();

		void ForcefullyClosedServerSocket();

		void ProjectAStartFailure(string reasonForFailure);

		void DaemonStartFailure(string reasonForFailure);

		void StartingApkInstall(string correlationId, string apkFileName);

		void StartingApkSync(string correlationId, string apkFileName);

		void ApkSyncSuccess(string correlationId);

		void ApkSyncFailure(string correlationId, string reasonForFailure);

		void ApkManifestDecoding(string correlationId);

		void ApkManifestDecoded(string correlationId);

		void ApkResourcesDecoding(string correlationId);

		void ApkResourcesDecoded(string correlationId);

		void ApkConverting(string correlationId);

		void ApkConverted(string correlationId);

		void ApkConversionFailure(string correlationId, string reasonForFailure);

		void ApkManifestInfo(string manifestInfoPackageName, string correlationId);

		void StartAppxInstall(string correlationId);

		void AppxInstalled(string correlationId);

		void AppxInstallFailure(string correlationId, string reasonForFailure);

		void AppxInstallReattempt();

		void AppxUninstalled(string correlationId);

		void AppxUninstallFailure(string correlationId, string reasonForFailure);

		void OpenedDaemonChannel(uint localId, string channelName);

		void OpenDaemonChannelFailure(uint originalLocalId, string channelName);

		void TooManyPendingChannelJobs();

		void AdbComandSent(uint command, uint arg0, uint arg1, byte[] data);

		void LogInfo(string message);

		void LogMessage(string message);

		void LogError(string errorMessage);

		void LogWarning(string warning);
	}
}
