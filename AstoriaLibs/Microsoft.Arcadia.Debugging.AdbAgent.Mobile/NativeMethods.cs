using System;
using System.Runtime.InteropServices;

namespace Microsoft.Arcadia.Debugging.AdbAgent.Mobile
{
	internal static class NativeMethods
	{
		public enum TOKEN_INFORMATION_CLASS
		{
			TokenUser = 1,
			TokenGroups,
			TokenPrivileges,
			TokenOwner,
			TokenPrimaryGroup,
			TokenDefaultDacl,
			TokenSource,
			TokenType,
			TokenImpersonationLevel,
			TokenStatistics,
			TokenRestrictedSids,
			TokenSessionId,
			TokenGroupsAndPrivileges,
			TokenSessionReference,
			TokenSandBoxInert,
			TokenAuditPolicy,
			TokenOrigin,
			TokenElevationType,
			TokenLinkedToken,
			TokenElevation,
			TokenHasRestrictions,
			TokenAccessInformation,
			TokenVirtualizationAllowed,
			TokenVirtualizationEnabled,
			TokenIntegrityLevel,
			TokenUIAccess,
			TokenMandatoryPolicy,
			TokenLogonSid,
			MaxTokenInfoClass
		}

		public struct SID_AND_ATTRIBUTES
		{
			public IntPtr Sid;

			public uint Attributes;
		}

		public struct TOKEN_USER
		{
			public SID_AND_ATTRIBUTES User;
		}

		public const int TOKEN_QUERY = 8;

		[DllImport("api-ms-win-core-processthreads-l1-1-2.dll")]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool OpenProcessToken(IntPtr ProcessHandle, int DesiredAccess, out IntPtr TokenHandle);

		[DllImport("api-ms-win-core-processthreads-l1-1-2.dll")]
		public static extern IntPtr GetCurrentProcess();

		[DllImport("api-ms-win-security-base-l1-2-0.dll")]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool GetTokenInformation(IntPtr hToken, TOKEN_INFORMATION_CLASS tokenInfoClass, IntPtr TokenInformation, int tokeInfoLength, out int reqLength);

		[DllImport("api-ms-win-core-handle-l1-1-0.dll")]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool CloseHandle(IntPtr handle);

		[DllImport("api-ms-win-security-sddl-l1-1-0.dll", CharSet = CharSet.Unicode)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool ConvertSidToStringSid(IntPtr pSID, [MarshalAs(UnmanagedType.LPTStr)] out string pStringSid);

		[DllImport("ShellChromeAPI.dll", EntryPoint = "Shell_IsLocked", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool ShellIsLocked();
	}
}
