using System.Collections.Generic;

namespace Microsoft.Arcadia.Marketplace.PackageObjectModel.Apk
{
	public class Middleware
	{
		public MiddlewareInfo Info { get; private set; }

		public IReadOnlyCollection<string> DevCodeApisCallingMiddleware { get; private set; }

		public Middleware(MiddlewareInfo middlewareInfo, IReadOnlyCollection<string> devCodeApisCallingMiddleware)
		{
			Info = middlewareInfo;
			DevCodeApisCallingMiddleware = devCodeApisCallingMiddleware;
		}
	}
}