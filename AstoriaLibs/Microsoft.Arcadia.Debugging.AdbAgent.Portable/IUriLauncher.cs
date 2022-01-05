using System;
using System.Threading.Tasks;

namespace Microsoft.Arcadia.Debugging.AdbAgent.Portable
{
	public interface IUriLauncher
	{
		Task LaunchUri(Uri uri);
	}
}