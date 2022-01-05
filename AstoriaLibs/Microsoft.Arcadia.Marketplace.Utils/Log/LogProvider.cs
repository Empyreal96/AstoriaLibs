namespace Microsoft.Arcadia.Marketplace.Utils.Log
{
	public abstract class LogProvider : ILogProvider
	{
		public LoggerCore.LogLevels LogLevels { get; set; }

		public LoggerCore.LogDecorations LogDecorations { get; set; }

		public virtual void InitLog()
		{
		}

		public abstract void DeinitLog();

		public abstract void Log(ILogMessage logMessage);
	}
}