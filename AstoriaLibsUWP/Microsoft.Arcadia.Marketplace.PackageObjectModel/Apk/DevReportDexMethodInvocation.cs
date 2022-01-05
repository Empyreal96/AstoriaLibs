using System;
using System.Collections.Generic;
using System.Globalization;

namespace Microsoft.Arcadia.Marketplace.PackageObjectModel.Apk
{
	public class DevReportDexMethodInvocation
	{
		private List<string> allParameters;

		public string MethodName { get; private set; }

		public string ParentMethod { get; private set; }

		public string ClassName { get; private set; }

		public IReadOnlyList<string> AllParameters => allParameters;

		public DevReportDexMethodInvocation(string className, string methodName, string parentMethod)
		{
			if (string.IsNullOrEmpty(className))
			{
				throw new ArgumentException("className must not be null or empty.", "className");
			}
			if (string.IsNullOrEmpty(methodName))
			{
				throw new ArgumentException("methodName must not be null or empty.", "methodName");
			}
			ClassName = className;
			MethodName = methodName;
			ParentMethod = parentMethod;
			allParameters = new List<string>();
		}

		public void AddParameter(string parameterValue)
		{
			if (parameterValue == null)
			{
				throw new ArgumentNullException("parameterValue");
			}
			if (string.IsNullOrWhiteSpace(parameterValue))
			{
				throw new ArgumentException("A valid parameter value must be provided.", "parameterValue");
			}
			allParameters.Add(parameterValue);
		}

		public override string ToString()
		{
			return string.Format(CultureInfo.InvariantCulture, "{0}.{1}({2}) ({3})", ClassName, MethodName, string.Join(", ", AllParameters), ParentMethod);
		}
	}
}