using System;
using System.Threading.Tasks;
using Microsoft.Arcadia.Debugging.AdbAgent.Portable;
using Microsoft.Arcadia.Debugging.AdbAgent.Portable.Exceptions;
using Windows.System;

namespace Microsoft.Arcadia.Debugging.AdbAgent.Mobile
{
	public class UriLauncherMobile : IUriLauncher
	{
		public async Task LaunchUri(Uri uri)
		{
			if (uri == null)
			{
				throw new ArgumentNullException("uri");
			}
			try
			{
				if (!(await Launcher.LaunchUriAsync(uri)))
				{
					throw new LauncherUriException();
				}
			}
			catch (LauncherUriException)
			{
				throw;
			}
			catch (Exception ex2)
			{
				throw new LauncherUriException(ex2);
			}
		}
	}
}
