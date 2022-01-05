using System;

namespace Microsoft.Arcadia.Debugging.AdbAgent.Portable
{
	internal static class NativeMethods
	{
		private const int ErrorHandleDiskFull = 39;

		private const int ErrorDiskFull = 112;

		public static bool IsDiskspaceFullException(Exception ex)
		{
			if (ex == null)
			{
				return false;
			}
			int num = ex.HResult & 0xFFFF;
			if (num != 39)
			{
				return num == 112;
			}
			return true;
		}
	}
}