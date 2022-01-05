namespace Microsoft.Arcadia.Marketplace.Utils.Log
{
	public interface ILogProvider
	{
		LoggerCore.LogLevels LogLevels { get; set; }

		LoggerCore.LogDecorations LogDecorations { get; set; }

		void InitLog();

		void DeinitLog();

		void Log(ILogMessage logMessage);
	}
}