using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

namespace Microsoft.Arcadia.Marketplace.Utils.Portable.SystemEX
{
	[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "Reviewed.")]
	internal class Win32NativeMethods
	{
		[SuppressMessage("Microsoft.StyleCop.CSharp.NamingRules", "SA1305:FieldNamesMustNotUseHungarianNotation", Justification = "Reviewed. Suppression is OK here.")]
		[SuppressMessage("Microsoft.StyleCop.CSharp.NamingRules", "SA1307:AccessibleFieldsMustBeginWithUpperCaseLetter", Justification = "Reviewed. Suppression is OK here.")]
		public struct PROCESS_INFORMATION
		{
			public IntPtr hProcessHandle;

			public IntPtr hThreadHandle;

			public int dwProcessId;

			public int dwThreadId;
		}

		[SuppressMessage("Microsoft.StyleCop.CSharp.NamingRules", "SA1305:FieldNamesMustNotUseHungarianNotation", Justification = "Reviewed. Suppression is OK here.")]
		[SuppressMessage("Microsoft.StyleCop.CSharp.NamingRules", "SA1307:AccessibleFieldsMustBeginWithUpperCaseLetter", Justification = "Reviewed. Suppression is OK here.")]
		public struct STARTUPINFO
		{
			public int cb;

			public string lpReserved;

			public string lpDesktop;

			public string lpTitle;

			public int dwX;

			public int dwY;

			public int dwXSize;

			public int dwYSize;

			public int dwXCountChars;

			public int dwYCountChars;

			public int dwFillAttribute;

			public int dwFlags;

			public short wShowWindow;

			public short cbReserved2;

			public IntPtr lpReserved2;

			public IntPtr hStdInput;

			public IntPtr hStdOutput;

			public IntPtr hStdError;
		}

		public const uint CREATE_NEW_CONSOLE = 16u;

		public const uint STILL_ACTIVE = 259u;

		public const uint WAIT_FAILED = uint.MaxValue;

		[DllImport("api-ms-win-core-processthreads-l1-1-2.dll", CharSet = CharSet.Unicode, SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool TerminateProcess(IntPtr processHandle, uint uExitCode);

		[DllImport("api-ms-win-core-synch-l1-2-0.dll", CharSet = CharSet.Unicode, SetLastError = true)]
		public static extern uint WaitForSingleObject(IntPtr handle, int dwMilliseconds);

		[DllImport("api-ms-win-core-processthreads-l1-1-2.dll", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool GetExitCodeProcess(IntPtr hProcess, out int lpExitCode);

		[DllImport("api-ms-win-core-processthreads-l1-1-2.dll", CharSet = CharSet.Unicode, SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool CreateProcess(string lpApplicationName, string lpCommandLine, IntPtr lpProcessAttributes, IntPtr lpThreadAttributes, [MarshalAs(UnmanagedType.Bool)] bool bInheritHandles, uint dwCreationFlags, IntPtr lpEnvironment, string lpCurrentDirectory, ref STARTUPINFO lpStartupInfo, out PROCESS_INFORMATION lpProcessInformation);
	}
}