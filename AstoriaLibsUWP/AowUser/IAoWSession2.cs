using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace AowUser
{
	[ComImport]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	[Guid("8AB860CB-32FF-4903-8F2A-5CA76ED80301")]
	public interface IAoWSession2
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		void StartInstanceForWindowsApp([In][ComAliasName("AowUser.GUID")] ref GUID instanceIid, out IntPtr ppInstance);

		[MethodImpl(MethodImplOptions.InternalCall)]
		void StartDefaultInstance([In][ComAliasName("AowUser.GUID")] ref GUID instanceIid, out IntPtr ppInstance);
	}
}