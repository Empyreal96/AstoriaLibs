namespace Microsoft.Arcadia.Marketplace.Utils.Log
{
	public interface ILogMessage
	{
		LoggerCore.LogLevels LogLevel { get; }

		string GetLogMessage(LoggerCore.LogDecorations logDecoration, LoggerCore.LogLevels logLevel);
	}
}