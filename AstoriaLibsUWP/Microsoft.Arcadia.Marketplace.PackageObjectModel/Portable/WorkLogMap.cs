using System;
using System.Collections.Concurrent;
using System.Reflection;

namespace Microsoft.Arcadia.Marketplace.PackageObjectModel.Portable
{
	public sealed class WorkLogMap
	{
		private static WorkLogMap instance;

		private static object instanceLock = new object();

		private ConcurrentDictionary<WorkerLogKey, LogAnnotationAttribute> logMap;

		public static WorkLogMap Instance
		{
			get
			{
				lock (instanceLock)
				{
					if (instance == null)
					{
						instance = new WorkLogMap();
					}
					return instance;
				}
			}
		}

		private WorkLogMap()
		{
			logMap = new ConcurrentDictionary<WorkerLogKey, LogAnnotationAttribute>();
			Type typeFromHandle = typeof(WorkerLogKey);
			foreach (WorkerLogKey value in Enum.GetValues(typeFromHandle))
			{
				FieldInfo runtimeField = typeFromHandle.GetRuntimeField(value.ToString());
				LogAnnotationAttribute customAttribute = runtimeField.GetCustomAttribute<LogAnnotationAttribute>();
				if (customAttribute != null)
				{
					customAttribute.EnumName = value.ToString();
					logMap.TryAdd(value, customAttribute);
				}
			}
		}

		public LogAnnotationAttribute GetLogData(WorkerLogKey logKey)
		{
			return logMap[logKey];
		}
	}
}