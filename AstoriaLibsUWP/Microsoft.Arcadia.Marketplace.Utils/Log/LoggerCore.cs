using System;
using System.Collections.Generic;
using System.Globalization;

namespace Microsoft.Arcadia.Marketplace.Utils.Log
{
	public static class LoggerCore
	{
		[Flags]
		public enum LogDecorations
		{
			None = 0,
			Detail = 1,
			Time = 2,
			LogLevel = 4,
			All = 0xFF
		}

		[Flags]
		public enum LogLevels
		{
			None = 0,
			Debug = 1,
			Info = 2,
			Warning = 4,
			Error = 8,
			Exp = 0x10,
			ExpStack = 0x20,
			All = 0xFF
		}

		private static readonly object SyncObject = new object();

		private static List<LogProvider> logProviders;

		private static ILogMessageFormatter formatter;

		public static LogProvider GetLogProvider(Type providerType)
		{
			lock (SyncObject)
			{
				if (logProviders != null)
				{
					foreach (LogProvider logProvider in logProviders)
					{
						if ((object)logProvider.GetType() == providerType)
						{
							return logProvider;
						}
					}
				}
			}
			return null;
		}

		public static void Log(LogLevels logLevel, string message, IMessageArg[] messageArgs)
		{
			DoLog(logLevel, message, messageArgs);
		}

		public static void Log(LogLevels logLevel, string message, IMessageArg messageArg)
		{
			DoLog(logLevel, message, new IMessageArg[1] { messageArg });
		}

		public static void Log(LogLevels logLevel, string format, params object[] args)
		{
			DoLog(logLevel, string.Format(CultureInfo.InvariantCulture, format, args), null);
		}

		public static void Log(LogLevels logLevel, string message)
		{
			DoLog(logLevel, message, null);
		}

		public static void Log(string message, IMessageArg[] messageArgs)
		{
			DoLog(LogLevels.Debug, message, messageArgs);
		}

		public static void Log(string message, IMessageArg messageArgs)
		{
			DoLog(LogLevels.Debug, message, new IMessageArg[1] { messageArgs });
		}

		public static void Log(string message)
		{
			DoLog(LogLevels.Debug, message, null);
		}

		public static void Log(string format, params object[] args)
		{
			DoLog(LogLevels.Debug, string.Format(CultureInfo.InvariantCulture, format, args), null);
		}

		public static void Log(Exception exp)
		{
			if (exp != null)
			{
				string exceptionMessage = formatter.GetExceptionMessage(exp);
				DoLog(LogLevels.Exp, exceptionMessage, new IMessageArg[1]
				{
				new ExpMessageArg(exp)
				});
			}
		}

		public static void Log(LogLevels logLevel, Exception exp)
		{
			DoLog(logLevel, string.Empty, new IMessageArg[1]
			{
			new ExpMessageArg(exp)
			});
		}

		public static void Log()
		{
			DoLog(LogLevels.Debug, string.Empty, null);
		}

		public static void RemoveLogProvider(LogProvider logProvider)
		{
			if (logProvider == null)
			{
				throw new ArgumentNullException("logProvider");
			}
			lock (SyncObject)
			{
				if (logProviders != null)
				{
					logProviders.Remove(logProvider);
					logProvider.DeinitLog();
				}
			}
		}

		public static void RemoveLogProvider(Type providerType)
		{
			RemoveLogProvider(GetLogProvider(providerType));
		}

		public static void AddLogProvider(LogProvider logProvider)
		{
			if (logProvider == null)
			{
				throw new ArgumentNullException("logProvider");
			}
			lock (SyncObject)
			{
				if (logProviders == null)
				{
					logProviders = new List<LogProvider>();
				}
				if (GetLogProvider(logProvider.GetType()) == null)
				{
					logProvider.InitLog();
					logProviders.Add(logProvider);
				}
			}
		}

		public static void RegisterFormatter(ILogMessageFormatter newFormatter)
		{
			if (newFormatter == null)
			{
				throw new ArgumentNullException("newFormatter");
			}
			lock (SyncObject)
			{
				if (newFormatter != null)
				{
					formatter = newFormatter;
				}
			}
		}

		public static void Deinit()
		{
			RemoveProviders();
		}

		private static void ProvidersLog(ILogMessage logMessage)
		{
			if (logProviders == null)
			{
				return;
			}
			foreach (LogProvider logProvider in logProviders)
			{
				if ((logProvider.LogLevels & logMessage.LogLevel) != 0)
				{
					logProvider.Log(logMessage);
				}
			}
		}

		private static bool CanSkipThisLogLevel(LogLevels logLevel)
		{
			if (logProviders != null)
			{
				foreach (LogProvider logProvider in logProviders)
				{
					if ((logProvider.LogLevels & logLevel) != 0)
					{
						return false;
					}
				}
			}
			return true;
		}

		private static void DoLog(LogLevels logLevel, string message, IMessageArg[] messageArgs)
		{
			lock (SyncObject)
			{
				if (!CanSkipThisLogLevel(logLevel))
				{
					ILogMessage logMessage = formatter.CreateMessage(logLevel, message, messageArgs);
					ProvidersLog(logMessage);
				}
			}
		}

		private static void RemoveProviders()
		{
			lock (SyncObject)
			{
				if (logProviders != null)
				{
					foreach (LogProvider logProvider in logProviders)
					{
						logProvider.DeinitLog();
					}
					logProviders.Clear();
				}
				logProviders = null;
			}
		}
	}
}