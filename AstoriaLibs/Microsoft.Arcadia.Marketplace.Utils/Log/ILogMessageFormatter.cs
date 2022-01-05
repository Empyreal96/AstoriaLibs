using System;

namespace Microsoft.Arcadia.Marketplace.Utils.Log
{
	public interface ILogMessageFormatter
	{
		ILogMessage CreateMessage(LoggerCore.LogLevels logLevel, string message, IMessageArg[] messageArgs);

		string GetExceptionMessage(Exception exception);
	}
}