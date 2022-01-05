using System.Globalization;

namespace Microsoft.Arcadia.Marketplace.PackageObjectModel.Apk
{
	public class CalledTrackedServiceMethodDetails
	{
		public string ServiceName { get; private set; }

		public string ServiceMethodPackageName { get; private set; }

		public string ServiceMethodCategory { get; private set; }

		public string ServiceMethodClassName { get; private set; }

		public string ServiceMethodName { get; private set; }

		public string CalledByMiddleware { get; private set; }

		public ClassMemberStatus ServiceMethodStatus { get; private set; }

		public string MethodNameCallingThisServiceMethod { get; private set; }

		public CalledTrackedServiceMethodDetails(string serviceName, string serviceMethodPackageName, string serviceMethodCategory, string serviceMethodClassName, string serviceMethodName, string calledByMiddleware, ClassMemberStatus serviceMethodStatus, string methodNameCallingThisServiceMethod)
		{
			ServiceName = serviceName;
			ServiceMethodPackageName = serviceMethodPackageName;
			ServiceMethodCategory = serviceMethodCategory;
			ServiceMethodClassName = serviceMethodClassName;
			ServiceMethodName = serviceMethodName;
			CalledByMiddleware = calledByMiddleware;
			ServiceMethodStatus = serviceMethodStatus;
			MethodNameCallingThisServiceMethod = methodNameCallingThisServiceMethod;
		}

		public override string ToString()
		{
			string text = string.Format(CultureInfo.InvariantCulture, "{0}.{1}.{2}", new object[3] { ServiceMethodPackageName, ServiceMethodClassName, ServiceMethodName });
			if (string.IsNullOrWhiteSpace(CalledByMiddleware))
			{
				text = string.Format(CultureInfo.InvariantCulture, "{0} ({1})", new object[2] { text, CalledByMiddleware });
			}
			return text;
		}
	}
}