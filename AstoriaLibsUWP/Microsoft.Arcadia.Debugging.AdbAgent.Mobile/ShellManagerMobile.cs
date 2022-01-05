using Microsoft.Arcadia.Debugging.AdbAgent.Portable;

namespace Microsoft.Arcadia.Debugging.AdbAgent.Mobile
{
	public class ShellManagerMobile : IShellManager
	{
		public bool IsScreenLocked => NativeMethods.ShellIsLocked();
	}
}
