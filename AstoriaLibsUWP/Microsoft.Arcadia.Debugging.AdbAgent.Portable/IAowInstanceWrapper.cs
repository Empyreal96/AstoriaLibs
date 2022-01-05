using System;

namespace Microsoft.Arcadia.Debugging.AdbAgent.Portable
{
	public interface IAowInstanceWrapper : IDisposable
	{
		void StartAow();

		void ReleaseAow();

		string AndroidPackageToWindowsPackage(string androidPackageName);
	}
}