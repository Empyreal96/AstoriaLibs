using System;
using System.IO;
using System.Runtime.InteropServices;
using Microsoft.Arcadia.Marketplace.Utils.Log;

namespace Microsoft.Arcadia.Marketplace.Utils.Portable.SystemEX
{
	public class PortableProcessRunner : ProcessRunnerBase
	{
		private Win32NativeMethods.PROCESS_INFORMATION procInfo;

		private Win32NativeMethods.STARTUPINFO startUpInfo;

		protected override void OnLaunchProcess()
		{
			procInfo = default(Win32NativeMethods.PROCESS_INFORMATION);
			startUpInfo = default(Win32NativeMethods.STARTUPINFO);
			LoggerCore.Log("Executing: {0} {1}.", base.ExePath, base.Arguments);
			bool flag = Win32NativeMethods.CreateProcess(base.ExePath, " " + base.Arguments, IntPtr.Zero, IntPtr.Zero, bInheritHandles: false, 16u, IntPtr.Zero, Path.GetDirectoryName(base.ExePath), ref startUpInfo, out procInfo);
			base.HasStarted = true;
			if (!flag)
			{
				int lastWin32Error = Marshal.GetLastWin32Error();
				throw new Win32InteropException("Could not create process.", lastWin32Error);
			}
		}

		protected override bool OnWaitForExitOrTimeout(int timeoutMilliseconds)
		{
			uint num = Win32NativeMethods.WaitForSingleObject(procInfo.hProcessHandle, timeoutMilliseconds);
			if (num == uint.MaxValue)
			{
				int lastWin32Error = Marshal.GetLastWin32Error();
				throw new Win32InteropException("Could not wait on process.", lastWin32Error);
			}
			int lpExitCode = 0;
			if (Win32NativeMethods.GetExitCodeProcess(procInfo.hProcessHandle, out lpExitCode))
			{
				if ((long)lpExitCode == 259)
				{
					return false;
				}
				base.ExitCode = lpExitCode;
				base.HasFinished = true;
				return true;
			}
			int lastWin32Error2 = Marshal.GetLastWin32Error();
			throw new Win32InteropException("Could not get exit code for process.", lastWin32Error2);
		}

		protected override void OnTerminateRunningProcess()
		{
			if (!Win32NativeMethods.TerminateProcess(procInfo.hProcessHandle, 0u))
			{
				LoggerCore.Log("Error terminating process with identifier {0}.", procInfo.hProcessHandle);
			}
		}
	}
}